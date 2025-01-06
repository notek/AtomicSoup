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
    [DIProvide(typeof(AtomHashSet), "AtomsChangePublished")]
    [DIProvide(typeof(SubscribersByAtomsDictionary), "ViewSubscribers")]
    [DIProvide(typeof(SubscribersByAtomsDictionary), "ContextSubscribers")]
    [DIProvide(typeof(SubscriberHashSet), "PublishingSubscribers")]
    public abstract class AtomDistributor : UdonSharpBehaviour
    {
        [SerializeField, DIInject("ViewSubscribers")] protected SubscribersByAtomsDictionary _ViewSubscribers;
        [SerializeField, DIInject("ContextSubscribers")] protected SubscribersByAtomsDictionary _ContextSubscribers;
        [SerializeField, DIInject("AtomsChanged")] protected AtomHashSet _AtomsChanged;
        [SerializeField, DIInject("AtomsChangePublished")] protected AtomHashSet _AtomsChangePublished;
        [SerializeField, DIInject("PublishingSubscribers")] protected SubscriberHashSet _PublishingSubscribers;
        protected abstract DistributorConsistency Consistency { get; }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void ClearSubscribers()
        {
            _ViewSubscribers?.Clear();
            _ContextSubscribers?.Clear();
        }

        public void SubscribeOnChangeForView(AtomSubscriber subscriber, params Atom[] atoms)
        {
            if (_ViewSubscribers == null)
                return;
            _ViewSubscribers.Register(subscriber, atoms);
        }

        public void SubscribeOnChangeForContext(AtomSubscriber subscriber, params Atom[] atoms)
        {
            if (_ContextSubscribers == null)
                return;
            _ContextSubscribers.Register(subscriber, atoms);
        }
#endif

        void Update()
        {
            //TODO: 不要なイベントが飛ぶ
            PublishAtomValuesToContext();
            //TODO: 非同期AtomはDirtyをチェックして待機する
            PublishAtomValuesToView();
        }

        public void OnAtomChanged(Atom atom)
        {
            if (Consistency == DistributorConsistency.Eventual)
            {
                atom.ReflectNextValue();
            }
            _AtomsChanged.Add(atom);
        }

        public void PublishAtomValuesToContext()
        {
            while (_AtomsChanged.Count() > 0)
            {
                var atomsPublishing = _AtomsChanged.ToArray();
                _AtomsChanged.Clear();
                foreach (var atomPublishing in atomsPublishing)
                {
                    var published = _AtomsChangePublished.Contains(atomPublishing);
                    if (!published)
                    {
                        atomPublishing.ReflectNextValue();
                        foreach (var subscriber in _ContextSubscribers.Get(atomPublishing))
                        {
                            _PublishingSubscribers.Add(subscriber);
                        }
                        _AtomsChangePublished.Add(atomPublishing);
                    }
                }

                foreach (var subscriber in _PublishingSubscribers.ToArray())
                {
                    subscriber.OnChange();
                }
                _PublishingSubscribers.Clear();
            }
        }
        void PublishAtomValuesToView()
        {
            var viewsPublishing = _AtomsChangePublished.ToArray();
            _AtomsChangePublished.Clear();
            foreach (var view in viewsPublishing)
            {
                foreach (var subscriber in _ViewSubscribers.Get(view))
                {
                    _PublishingSubscribers.Add(subscriber);
                }
            }
            foreach (var subscriber in _PublishingSubscribers.ToArray())
            {
                subscriber.OnChange();
            }
            _PublishingSubscribers.Clear();
        }
    }
}