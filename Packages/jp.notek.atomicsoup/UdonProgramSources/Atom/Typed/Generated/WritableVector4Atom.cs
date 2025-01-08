
/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{
    public abstract class WritableVector4Atom : Vector4Atom
    {
        public override bool IsInputNode { get { return true; } }
        public override void OnChange() { }

        public void Set(Vector4 value)
        {
            SetNextValue(value);
            _Distributor.OnInputNodeChanged(this);
        }
    }
}
