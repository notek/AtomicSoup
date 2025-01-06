using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace JP.Notek.AtomicSoup
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class IReduceStore : UdonSharpBehaviour
    {
        public bool IsStateDistributing = false;
        public virtual void Reduce(string action, DataToken value) { }
        public virtual void Reduce(string action, VRCUrl value) { }
    }
}