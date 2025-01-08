using UdonSharp;
using VRC.SDK3.Data;
using VRC.SDKBase;
namespace JP.Notek.AtomicSoup.VRCCollection
{
    [UdonSharpProgramAsset]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VRCUrlDictionary : UdonSharpBehaviour
    {
        DataDictionary _Dictionary = new DataDictionary();
        VRCUrl[] _Values = new VRCUrl[0];

        public void Set(DataToken key, VRCUrl value)
        {
            if (_Dictionary.ContainsKey(key))
            {
                _Values[(int)_Dictionary[key]] = value;
            }
            else
            {
                _Dictionary.Add(key, _Values.Length);
                _Values = _Values.Concat(value);
            }
        }
        public void Remove(DataToken key)
        {
            _Values[(int)_Dictionary[key]] = VRCUrl.Empty;
            _Dictionary.Remove(key);
        }
        public bool Contains(DataToken key)
        {
            return _Dictionary.ContainsKey(key);
        }
        public DataToken[] GetKeys()
        {
            return _Dictionary.GetKeys().ToArray();
        }
        public void Clear()
        {
            _Dictionary.Clear();
        }
        public int Count()
        {
            return _Dictionary.Count;
        }
        public VRCUrl Get(DataToken key)
        {
            if (_Dictionary.ContainsKey(key))
            {
                return _Values[(int)_Dictionary[key]];
            }
            return VRCUrl.Empty;
        }
        public VRCUrl this[DataToken key]
        {
            get => Get(key);
            set => Set(key, value);
        }
    }
}