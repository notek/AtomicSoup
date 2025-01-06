using System;
using UdonSharp;

namespace JP.Notek.AtomicSoup
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DIInjectAttribute : Attribute
    {
        public string Id { get; private set; }

        public DIInjectAttribute()
        {
            Id = null;
        }
        public DIInjectAttribute(string id)
        {
            Id = id;
        }
    }
}