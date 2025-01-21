using System;
using UdonSharp;

namespace JP.Notek.AtomicSoup
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DIInjectAttribute : Attribute
    {
        public string Id { get; private set; }
        public Type Type { get; private set; }

        public DIInjectAttribute()
        {
            Id = null;
            Type = null;
        }
        public DIInjectAttribute(string id)
        {
            Id = id;
            Type = null;
        }
        public DIInjectAttribute(Type type)
        {
            Id = null;
            Type = type;
        }
        public DIInjectAttribute(string id, Type type)
        {
            Id = id;
            Type = type;
        }
    }
}