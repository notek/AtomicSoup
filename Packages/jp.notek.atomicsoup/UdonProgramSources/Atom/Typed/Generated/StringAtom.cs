
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
    public abstract class StringAtom : AtomSubscriberForAtom
    {
        protected abstract string _Value { get; set; }
        public string Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected string _PreviousValue;
        public string PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected string _NextValue;
        public string NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(string value)
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
