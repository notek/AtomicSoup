
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
    public abstract class UshortAtom : AtomSubscriberForAtom
    {
        protected abstract ushort _Value { get; set; }
        public ushort Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected ushort _PreviousValue;
        public ushort PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected ushort _NextValue;
        public ushort NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(ushort value)
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
