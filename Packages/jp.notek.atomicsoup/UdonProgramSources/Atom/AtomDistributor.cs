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

        AtomSubscriber[] _SecondaryQueue = new AtomSubscriber[0];

        protected abstract DistributorConsistency Consistency { get; }
        bool _Lock = false;

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
            PublishPrimary();
            PublishSecondary();
        }

        public void OnInputNodeChanged(AtomSubscriber atom)
        {
            if (Consistency == DistributorConsistency.Eventual)
            {
                atom.ReflectNextValue();
            }
            _AtomsChanged.Add(atom);
        }

        public void PublishPrimary()
        {
            if(_AtomsChanged.Count() == 0)
                return;
            if(_SecondaryQueue.Length > 0)
            {
                return;
            }
            if (_Lock)
                return;
            _Lock = true;
            var primaryQueue = _GraphManager.GetPrimaryQueue(_AtomsChanged);
            var secondaryQueue = _GraphManager.GetSecondaryQueue(_AtomsChanged);
            var atomsChanged =_AtomsChanged.ToArray();
            foreach (var atom in atomsChanged)
            {
                atom.ReflectNextValue();
            }
            _AtomsChanged.Clear();
            foreach (var atom in primaryQueue)
            {
                atom.OnChange();
                atom.ReflectNextValue();
            }

            _SecondaryQueue = secondaryQueue;
            _Lock = false;
        }
        void PublishSecondary()
        {
            if (_SecondaryQueue.Length == 0)
                return;
            if (_Lock)
                return;
            _Lock = true;
            //TODO: 非同期Atomは更新完了後にDirtyフラグを戻す

            foreach (var atom in _SecondaryQueue)
            {
                atom.OnChange();
            }
            _SecondaryQueue = new AtomSubscriber[0];
            _Lock = false;
        }
    }
}