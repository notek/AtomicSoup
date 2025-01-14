using System;
using System.ComponentModel;
using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup.Pipe
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class PipeTask : UdonSharpBehaviour
    {
        protected string _ExecutingJobID = null;

        DataDictionary _ReceiverByJobID = new DataDictionary();
        UdonSharpBehaviour[] _Receiver = new UdonSharpBehaviour[0];
        string[] _ReceiverNextCallbackName = new string[0];
        string[] _ReceiverErrorCallbackName = new string[0];
        protected string IssueJob(UdonSharpBehaviour worker, string nextCallbackName, string errorCallbackName)
        {
            var executingJobID = Guid.NewGuid().ToString();
            _Receiver = _Receiver.Concat(worker);
            _ReceiverNextCallbackName = _ReceiverNextCallbackName.Concat(nextCallbackName);
            _ReceiverErrorCallbackName = _ReceiverErrorCallbackName.Concat(errorCallbackName);
            _ReceiverByJobID.Add(executingJobID, _Receiver.Length - 1);
            _ExecutingJobID = executingJobID;
            return executingJobID;
        }

        public virtual void Cancel(string jobID)
        {
            if (_ExecutingJobID == jobID)
            {
                _ExecutingJobID = null;
            }
            if (_ReceiverByJobID.ContainsKey(jobID))
            {
                UnsubscribeReceiver(jobID);
            }
        }
        private void UnsubscribeReceiver(string jobID)
        {
            var index = (int)_ReceiverByJobID[jobID];
            _Receiver = _Receiver.RemoveAt(index);
            _ReceiverNextCallbackName = _ReceiverNextCallbackName.RemoveAt(index);
            _ReceiverErrorCallbackName = _ReceiverErrorCallbackName.RemoveAt(index);
            _ReceiverByJobID.Remove(jobID);
        }
        public abstract void ClearResult(string jobID);

        protected void SendNext()
        {
            var worker = _Receiver[(int)_ReceiverByJobID[_ExecutingJobID]];
            var callbackName = _ReceiverNextCallbackName[(int)_ReceiverByJobID[_ExecutingJobID]];
            UnsubscribeReceiver(_ExecutingJobID);
            _ExecutingJobID = null;
            worker.SendCustomEvent(callbackName);
        }
        protected void SendError()
        {
            var worker = _Receiver[(int)_ReceiverByJobID[_ExecutingJobID]];
            var callbackName = _ReceiverErrorCallbackName[(int)_ReceiverByJobID[_ExecutingJobID]];
            UnsubscribeReceiver(_ExecutingJobID);
            _ExecutingJobID = null;
            worker.SendCustomEvent(callbackName);
        }
    }
}