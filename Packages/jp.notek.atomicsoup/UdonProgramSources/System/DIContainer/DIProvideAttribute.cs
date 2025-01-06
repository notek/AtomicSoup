using System;
using UnityEngine;
using UdonSharp;

namespace JP.Notek.AtomicSoup
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class DIProvideAttribute : Attribute
    {
        public Type ComponentType { get; private set; }
        public string Id { get; private set; }

        public DIProvideAttribute(Type componentType, string id = null)
        {
            ComponentType = componentType;
            Id = id;
        }
    }
}