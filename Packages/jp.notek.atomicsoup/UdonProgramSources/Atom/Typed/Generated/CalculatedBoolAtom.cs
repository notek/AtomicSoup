
/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{
    public abstract class CalculatedBoolAtom : BoolAtom
    {
        public abstract bool Factory();
        
        public override void OnChange()
        {
            SetNextValue(Factory());
        }
    }
}
