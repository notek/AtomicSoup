using UdonSharp;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup.VRCCollection
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class SubscriberHashSet : UdonSharpBehaviour
    {
        protected DataDictionary _SetDictionary = new DataDictionary();

        AtomSubscriber[] _Values = new AtomSubscriber[0];
        public void Add(AtomSubscriber key)
        {
            if (_SetDictionary.ContainsKey(key))
            {
                _Values[(int)_SetDictionary[key]] = key;
            }
            else
            {
                _SetDictionary.Add(key, _Values.Length);
                _Values = _Values.Add(key);
            }
        }
        public void Remove(AtomSubscriber key)
        {
            var index = (int)_SetDictionary[key];
            _Values.RemoveAt(index);
            _SetDictionary.Remove(key);
            foreach(var value in _Values)
            {
                var currentIndex = (int)_SetDictionary[value];
                if(currentIndex > index)
                {
                    _SetDictionary[value] = currentIndex - 1;
                }
            }
        }

        public AtomSubscriber[] ToArray()
        {
            return _Values;
        }

        public bool Contains(AtomSubscriber key)
        {
            return _SetDictionary.ContainsKey(key);
        }

        public void Clear()
        {
            _SetDictionary.Clear();
            _Values = new AtomSubscriber[0];
        }

        public int Count()
        {
            return _SetDictionary.Count;
        }
    }
}