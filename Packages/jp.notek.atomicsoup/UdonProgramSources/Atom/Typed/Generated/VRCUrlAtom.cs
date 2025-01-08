
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
    public abstract class VRCUrlAtom : AtomSubscriberForAtom
    {
        protected abstract VRCUrl _Value { get; set; }
        public VRCUrl Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected VRCUrl _PreviousValue;
        public VRCUrl PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected VRCUrl _NextValue;
        public VRCUrl NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(VRCUrl value)
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
            _Distributor.PublishPrimary();
        }
    }
}
