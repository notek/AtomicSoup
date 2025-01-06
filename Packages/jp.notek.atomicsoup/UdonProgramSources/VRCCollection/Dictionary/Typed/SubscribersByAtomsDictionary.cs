
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
namespace JP.Notek.AtomicSoup.VRCCollection
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SubscribersByAtomsDictionary : UdonSharpBehaviour
    {
        DataDictionary _Dictionary = new DataDictionary();
        AtomSubscriber[][] _Values = new AtomSubscriber[0][];

        [SerializeField] AtomSubscriber[] _AtomSubscribersFlatten = new AtomSubscriber[0];
        [SerializeField] Atom[] _AtomsFlatten = new Atom[0];

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        public void Clear()
        {
            _Dictionary.Clear();
            _AtomSubscribersFlatten = new AtomSubscriber[0];
            _AtomsFlatten = new Atom[0];
        }

        public void Register(AtomSubscriber subscriber, params Atom[] atoms)
        {
            foreach (var atom in atoms)
            {
                _AtomsFlatten = _AtomsFlatten.Add(atom);
                _AtomSubscribersFlatten = _AtomSubscribersFlatten.Add(subscriber);
            }
        }
#endif
        void Start()
        {
            Apply();
        }

        void Apply()
        {
            for (int i = 0; i < _AtomSubscribersFlatten.Length; i++)
            {
                var subscriber = _AtomSubscribersFlatten[i];
                var atom = _AtomsFlatten[i];
                if (!_Dictionary.ContainsKey(atom))
                {
                    var newSubscribersSet = new DataDictionary();
                    newSubscribersSet.Add("index", _Values.Length);
                    _Values = _Values.Add(new AtomSubscriber[0]);
                    _Dictionary.Add(atom, newSubscribersSet);
                }
                var subscribersSet = (DataDictionary)_Dictionary[atom];
                var values = _Values[(int)subscribersSet["index"]];
                if (subscribersSet.ContainsKey(subscriber))
                {
                    values[(int)subscribersSet[subscriber]] = subscriber;
                }
                else
                {
                    subscribersSet.Add(subscriber, values.Length);
                    _Values[(int)subscribersSet["index"]] = values.Add(subscriber);
                }
            }
        }
        public AtomSubscriber[] Get(Atom atom)
        {
            if (_Dictionary.ContainsKey(atom))
            {
                var subscribersSet = (DataDictionary)_Dictionary[atom];
                var index = (int)subscribersSet["index"];
                return _Values[index];
            }
            return new AtomSubscriber[0];
        }
    }
}