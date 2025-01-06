using UdonSharp;
using UnityEngine;

namespace JP.Notek.AtomicSoup
{
    public abstract class Atom : UdonSharpBehaviour
    {
        [SerializeField, DIInject] protected AtomDistributor _Distributor;
        public bool IsDirty { get; protected set; } = false;
        public abstract void ReflectNextValue();
    }
}