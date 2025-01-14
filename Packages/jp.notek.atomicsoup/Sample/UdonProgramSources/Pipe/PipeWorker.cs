using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
namespace JP.Notek.AtomicSoup.Sample
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PipeWorker : UdonSharpBehaviour
    {
        public RequestStringFromAPI _RequestTaskA;
        public string _RequestTaskAJobID;
        public RequestStringFromAPI _RequestTaskB;
        public string _RequestTaskBJobID;
        public void Execute()
        {
            Cancel();
            Task1();
        }
        void Task1()
        {
            // _RequestTaskAJobID = _RequestTaskA.Request(new VRCUrl("https://www.google.com"), this, "Task2", "Error");
        }
        void Task2()
        {
            // _RequestTaskBJobID = _RequestTaskB.Request(new VRCUrl("https://www.google.com"), this, "Complete", "Error2");
        }
        void Error2()
        {
            var expression = true;
            if (expression)
            {
                // Retry only Task2
                Task2();
            }
            else
            {
                Error();
            }
        }
        void Complete()
        {
            // Complete handling
            // Example:
            // Debug.Log(_RequestTaskA.GetResult(_RequestTaskAJobID));
            // Debug.Log(_RequestTaskB.GetResult(_RequestTaskBJobID));
            ClearResult();
        }
        void Error()
        {
            // Error handling
            ClearResult();

            // Retry this pipeline
            Execute();
        }
        void Cancel()
        {
            if (_RequestTaskAJobID != null)
                _RequestTaskA.Cancel(_RequestTaskAJobID);
            if (_RequestTaskBJobID != null)
                _RequestTaskB.Cancel(_RequestTaskBJobID);
            RemoveJobID();
        }
        void ClearResult()
        {
            if (_RequestTaskAJobID != null)
                _RequestTaskA.ClearResult(_RequestTaskAJobID);
            if (_RequestTaskBJobID != null)
                _RequestTaskB.ClearResult(_RequestTaskBJobID);
            RemoveJobID();
        }
        void RemoveJobID()
        {
            _RequestTaskAJobID = null;
            _RequestTaskBJobID = null;
        }
    }
}