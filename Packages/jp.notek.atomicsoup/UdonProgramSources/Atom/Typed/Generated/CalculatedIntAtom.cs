
/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{
    public abstract class CalculatedIntAtom : IntAtom
    {
        public abstract int Factory();
        
        public override void OnChange()
        {
            SetNextValue(Factory());
        }
    }
}
