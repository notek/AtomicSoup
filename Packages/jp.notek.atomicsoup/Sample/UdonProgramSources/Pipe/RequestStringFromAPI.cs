using System;
using JP.Notek.AtomicSoup;
using JP.Notek.AtomicSoup.Pipe;
using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace JP.Notek.AtomicSoup.Sample
{
    public enum RequestResult
    {
        Success,
        ParseError,
        Error
    }
    [UdonSharpProgramAsset]
    public class RequestStringFromAPI : PipeTask
    {
        public float RequestInterval = 3f;
        float _LastRequestTime = -1;
        VRCUrl _RequestingURL = null;

        DataDictionary _QueueByJobID = new DataDictionary();
        VRCUrl[] _QueueURL = new VRCUrl[0];

        DataDictionary _ResultByJobID = new DataDictionary();
        DataToken[] _ResultResponse = new DataToken[0];
        RequestResult[] _ResultStatus = new RequestResult[0];


        void Update()
        {
            if (_RequestingURL == null && _LastRequestTime + RequestInterval < Time.time)
            {
                var jobIDs = _QueueByJobID.GetKeys();
                if (jobIDs.Count > 0)
                {
                    _ExecutingJobID = (string)jobIDs[0];
                    _RequestingURL = _QueueURL[(int)_QueueByJobID[_ExecutingJobID]];
                    _QueueURL = _QueueURL.RemoveAt((int)_QueueByJobID[_ExecutingJobID]);
                    _QueueByJobID.Remove(_ExecutingJobID);
                    VRCStringDownloader.LoadUrl(_RequestingURL, (IUdonEventReceiver)this);
                    _LastRequestTime = Time.time;
                }
            }
        }
        public string Request(VRCUrl url, UdonSharpBehaviour receiver, string nextCallbackName, string errorCallbackName)
        {
            var requestID = IssueJob(receiver, nextCallbackName, errorCallbackName);
            _QueueURL = _QueueURL.Concat(url);
            _QueueByJobID.Add(requestID, _QueueURL.Length - 1);
            return requestID;
        }

        public override void Cancel(string jobID)
        {
            if (_QueueByJobID.ContainsKey(jobID))
            {
                _QueueURL = _QueueURL.RemoveAt((int)_QueueByJobID[jobID]);
                _QueueByJobID.Remove(jobID);
            }
            else if (_ExecutingJobID == jobID)
            {
                _RequestingURL = null;
            }
            base.Cancel(jobID);
        }

        public RequestResult GetResult(string jobID, out DataToken response)
        {
            var resultID = (int)_ResultByJobID[jobID];
            response = _ResultResponse[resultID];
            return _ResultStatus[resultID];
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            if (_ExecutingJobID == null || _RequestingURL.ToString() != result.Url.ToString())
            {
                _ExecutingJobID = null;
                _RequestingURL = null;
                return;
            }

            if (VRCJson.TryDeserializeFromJson(result.Result, out var response))
            {
                _ResultResponse = _ResultResponse.Concat(response);
                _ResultStatus = _ResultStatus.Concat(RequestResult.Success);
            }
            else
            {
                _ResultResponse = _ResultResponse.Concat(new DataToken());
                _ResultStatus = _ResultStatus.Concat(RequestResult.ParseError);
            }
            _ResultByJobID.Add(_ExecutingJobID, _ResultResponse.Length - 1);
            _RequestingURL = null;
            SendNext();
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            if (_ExecutingJobID == null || _RequestingURL.ToString() != result.Url.ToString())
            {
                _ExecutingJobID = null;
                _RequestingURL = null;
                return;
            }

            _ResultResponse = _ResultResponse.Concat(new DataToken());
            _ResultStatus = _ResultStatus.Concat(RequestResult.Error);
            _ResultByJobID.Add(_ExecutingJobID, _ResultResponse.Length - 1);
            _RequestingURL = null;
            SendError();
        }

        public override void ClearResult(string jobID)
        {
            _ResultResponse = _ResultResponse.RemoveAt((int)_ResultByJobID[jobID]);
            _ResultStatus = _ResultStatus.RemoveAt((int)_ResultByJobID[jobID]);
            _ResultByJobID.Remove(jobID);
        }
    }
}