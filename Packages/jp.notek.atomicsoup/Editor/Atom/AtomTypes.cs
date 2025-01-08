using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.Udon.Graph.NodeRegistries;

namespace JP.Notek.AtomicSoup.Editor
{
    public static class AtomTypes
    {
        public static readonly string[] Syncable = new[]
        {
            "byte", "sbyte", "double", "float", "int", "uint", "long", "ulong",
            "short", "ushort", "Color", "Color32", "Quaternion", "Vector2",
            "Vector3", "Vector4", "string", "char", "VRCUrl", "bool"
        };
        public static readonly string[] UdonDataContainer = new[]
        {
            "DataToken", "DataDictionary", "DataList"
        };
        private static readonly Dictionary<string, string> _typeToAtomTypeMap = new Dictionary<string, string>(
            from type in Syncable.Concat(UdonDataContainer)
            select new KeyValuePair<string, string>(type, $"{char.ToUpper(type[0])}{type.Substring(1)}Atom")
        );
        private static readonly Dictionary<string, string> _atomTypeToTypeMap = new Dictionary<string, string>(
            from pair in _typeToAtomTypeMap
            select new KeyValuePair<string, string>(pair.Value, pair.Key)
        );
        private static readonly HashSet<string> _syncableAtomTypeSet = new HashSet<string>(
            from type in Syncable.Concat(UdonDataContainer)
            select $"Syncable{char.ToUpper(type[0])}{type.Substring(1)}Atom"
        );


        public static string ToAtomType(string type)
        {
            return _typeToAtomTypeMap.TryGetValue(type, out var atomType) ? atomType : null;
        }
        public static string FromAtomType(string type)
        {
            return _atomTypeToTypeMap.TryGetValue(type, out var atomType) ? atomType : null;
        }

        public static bool IsAtomType(string type)
        {
            return _atomTypeToTypeMap.ContainsKey(type);
        }

        public static bool IsSyncableAtomType(string type)
        {
            return _syncableAtomTypeSet.Contains(type);
        }
    }
}