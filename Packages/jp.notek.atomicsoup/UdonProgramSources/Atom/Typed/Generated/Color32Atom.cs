
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
    public abstract class Color32Atom : AtomSubscriberForAtom
    {
        protected abstract Color32 _Value { get; set; }
        public Color32 Value { get {
            PublishIfUnchanged();
            return _Value;
        } }
        protected Color32 _PreviousValue;
        public Color32 PreviousValue { get {
            PublishIfUnchanged();
            return _PreviousValue;
        } }
        
        protected Color32 _NextValue;
        public Color32 NextValue { get {
            PublishIfUnchanged();
            return _NextValue;
        } }

        protected void SetNextValue(Color32 value)
        {
            if (_NextValue.Equals(value))
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
