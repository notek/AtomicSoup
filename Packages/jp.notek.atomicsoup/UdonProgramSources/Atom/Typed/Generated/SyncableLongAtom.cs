
/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{
    public abstract class SyncableLongAtom : LongAtom
    {
        public override bool IsInputNode { get { return true; } }
        public override void OnChange() { }

        public void Set(long value)
        {
            SetNextValue(value);
            _Distributor.OnInputNodeChanged(this);
        }
    }
}
