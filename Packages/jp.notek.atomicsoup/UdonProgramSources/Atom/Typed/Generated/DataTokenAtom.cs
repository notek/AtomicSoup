
/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class DataTokenAtom : AtomSubscriberForAtom
    {
        protected abstract DataToken _Value { get; set; }
        public DataToken Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected DataToken _PreviousValue;
        public DataToken PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected DataToken _NextValue;
        public DataToken NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(DataToken value)
        {
            if (_NextValue == value)
                return;
            _NextValue = value;
            IsDirty = true;
        }

        public override void ReflectNextValue()
        {
            _PreviousValue = _Value;
            _Value = _NextValue;
            IsDirty = false;
        }

        void PublishIfUnchanged()
        {
            _Distributor.DistributeIntermidiate();
        }
    }
}
