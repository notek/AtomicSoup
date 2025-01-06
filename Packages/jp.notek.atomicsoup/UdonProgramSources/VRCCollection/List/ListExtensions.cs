
using System;

namespace JP.Notek.AtomicSoup.VRCCollection
{
    public static class ListExtensions {
        public static T[] Add<T>(this T[] target, T source) {
            var ret = new T[target.Length + 1];
            Array.Copy(target, ret, target.Length);
            ret[ret.Length - 1] = source;
            return ret;
        }
        public static T Pop<T>(this T[] target, out T[] newArray) {
            var ret = target[0];
            newArray = new T[target.Length - 1];
            Array.Copy(target, 1, newArray, 0, target.Length - 1);
            return ret;
        }
        public static T[] RemoveAt<T>(this T[] target, int index) {
            var ret = new T[target.Length - 1];
            Array.Copy(target, 0, ret, 0, index);
            Array.Copy(target, index + 1, ret, index, target.Length - index - 1);
            return ret;
        }
    }
}