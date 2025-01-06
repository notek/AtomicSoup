using System;
using UnityEngine;
using UdonSharp;

namespace JP.Notek.AtomicSoup
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class SubscribeAtomAttribute : Attribute
    {
        public SubscribeAtomAttribute()
        { }
    }
}