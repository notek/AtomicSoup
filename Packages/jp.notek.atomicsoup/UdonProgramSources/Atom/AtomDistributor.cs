using JP.Notek.AtomicSoup.VRCCollection;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace JP.Notek.AtomicSoup
{
    public enum DistributorConsistency
    {
        Strong,
        Eventual
    }
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DIProvide(typeof(AtomHashSet), "AtomsChanged")]
    [DIProvide(typeof(AtomGraphManager))]
    [DIProvide(typeof(OwnerTimeDifferenceAtom))]
    public abstract class AtomDistributor : UdonSharpBehaviour
    {
        [SerializeField, DIInject] protected AtomGraphManager _GraphManager;
        [SerializeField, DIInject("AtomsChanged")] protected AtomHashSet _AtomsChanged;
        [DIInject] public OwnerTimeDifferenceAtom OwnerTimeDifference;

        AtomSubscriber[] _PrimaryQueue = new AtomSubscriber[0];
        AtomSubscriber[] _SecondaryQueue = new AtomSubscriber[0];

        protected abstract DistributorConsistency Consistency { get; }
        bool _Lock = false;
        int _PublishI = 0;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void ClearSubscribers()
        {
            _GraphManager?.Clear();
        }

        public void SubscribeOnChangeForView(AtomSubscriber subscriber, AtomSubscriber[] atoms)
        {
            _GraphManager?.RegisterViewOutputEdge(subscriber, atoms);
        }

        public void SubscribeOnChangeForContext(AtomSubscriber subscriber, AtomSubscriber[] atoms)
        {
            _GraphManager?.RegisterIntermidiateEdge(subscriber, atoms);
        }

        public void SubscribeOnChangeForSync(AtomSubscriber subscriber, AtomSubscriber[] atoms)
        {
            _GraphManager?.RegisterSyncOutputEdge(subscriber, atoms);
        }
        public void FinalizePreprocess()
        {
            _GraphManager?.RemoveDuplicateInputNodes();
        }
#endif

        void Start()
        {
            _GraphManager.Apply(Consistency);
        }

        void Update()
        {
            if (_Lock)
                return;
            _Lock = true;
            if (Consistency == DistributorConsistency.Strong)
                DistributeStrong();
            else
                DistributeEventual();
            _Lock = false;
        }

        public void OnInputNodeChanged(AtomSubscriber atom)
        {
            if (Consistency == DistributorConsistency.Eventual)
            {
                atom.ReflectNextValue();
            }
            _AtomsChanged.Add(atom);
        }

        public void DistributeStrong()
        {
            if (_SecondaryQueue.Length > 0)
                DistributeOutputNode();
            else
                DistributeIntermidiate();
        }

        public void DistributeEventual()
        {
            if (_PrimaryQueue.Length > 0)
            {
                _PrimaryQueue[_PublishI].OnChange();
                _PrimaryQueue[_PublishI++].ReflectNextValue();
                if (_PublishI >= _PrimaryQueue.Length)
                {
                    _PrimaryQueue = new AtomSubscriber[0];
                    _PublishI = 0;
                }
            }
            else
            {
                if (_AtomsChanged.Count() == 0)
                    return;
                _PrimaryQueue = _GraphManager.GetPrimaryQueue(_AtomsChanged);
                _AtomsChanged.Clear();
            }
        }

        public void DistributeIntermidiate()
        {
            if (_AtomsChanged.Count() == 0)
                return;
            if (_SecondaryQueue.Length > 0)
            {
                return;
            }
            _PrimaryQueue = _GraphManager.GetPrimaryQueue(_AtomsChanged);
            _SecondaryQueue = _GraphManager.GetSecondaryQueue(_AtomsChanged);
            var atomsChangedTaken = _AtomsChanged.ToArray();
            _AtomsChanged.Clear();
            foreach (var atom in atomsChangedTaken)
            {
                atom.ReflectNextValue();
            }
            foreach (var atom in _PrimaryQueue)
            {
                atom.OnChange();
                atom.ReflectNextValue();
            }
        }
        void DistributeOutputNode()
        {
            if (_SecondaryQueue.Length == 0)
                return;
            _SecondaryQueue[_PublishI++].OnChange();
            if (_PublishI >= _SecondaryQueue.Length)
            {
                _SecondaryQueue = new AtomSubscriber[0];
                _PublishI = 0;
            }
        }
    }
}