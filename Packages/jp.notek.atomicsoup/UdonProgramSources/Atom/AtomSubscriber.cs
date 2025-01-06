using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace JP.Notek.AtomicSoup
{
    public abstract class AtomSubscriber : Atom
    {
        public abstract void OnChange();
        public override void ReflectNextValue()
        { }
    }
}