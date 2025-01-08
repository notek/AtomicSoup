
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
    public abstract class ColorAtom : AtomSubscriberForAtom
    {
        protected abstract Color _Value { get; set; }
        public Color Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected Color _PreviousValue;
        public Color PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected Color _NextValue;
        public Color NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(Color value)
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
