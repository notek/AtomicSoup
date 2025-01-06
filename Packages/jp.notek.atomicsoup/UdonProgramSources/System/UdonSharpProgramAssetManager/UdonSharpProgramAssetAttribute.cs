using System;
using UnityEngine;
using UdonSharp;

namespace JP.Notek.AtomicSoup
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UdonSharpProgramAssetAttribute : Attribute
    {
        public UdonSharpProgramAssetAttribute()
        { }
    }
}