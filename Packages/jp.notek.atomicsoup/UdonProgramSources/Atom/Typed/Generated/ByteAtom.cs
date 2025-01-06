
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
    public abstract class ByteAtom : AtomSubscriberForAtom
    {
        protected abstract byte _Value { get; set; }
        public byte Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected byte _PreviousValue;
        public byte PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected byte _NextValue;
        public byte NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(byte value)
        {
            _NextValue = value;
            IsDirty = true;
            _Distributor.OnAtomChanged(this);
        }

        public override void ReflectNextValue()
        {
            _PreviousValue = _Value;
            _Value = _NextValue;
            IsDirty = false;
        }

        void PublishIfUnchanged()
        {
            _Distributor.PublishAtomValuesToContext();
        }
    }
}
