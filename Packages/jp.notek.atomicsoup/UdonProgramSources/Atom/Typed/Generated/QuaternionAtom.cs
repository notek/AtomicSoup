
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
    public abstract class QuaternionAtom : AtomSubscriberForAtom
    {
        protected abstract Quaternion _Value { get; set; }
        public Quaternion Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected Quaternion _PreviousValue;
        public Quaternion PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected Quaternion _NextValue;
        public Quaternion NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(Quaternion value)
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
