#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using System.Text;
using System.IO;
using System.Linq;
using UnityEngine;

namespace JP.Notek.AtomicSoup.Editor
{
    [InitializeOnLoad]
    public class AtomSourceGenerator
    {
        private static readonly string[] Types = new[]
        {
            "byte", "sbyte", "double", "float", "int", "uint", "long", "ulong",
            "short", "ushort", "Color", "Color32", "Quaternion", "Vector2",
            "Vector3", "Vector4", "string", "char", "VRCUrl", "bool"
        };
        private static readonly string[] UdonDataContainerTypes = new[]
        {
            "DataToken", "DataDictionary", "DataList"
        };
        static AtomSourceGenerator()
        {
            EditorApplication.delayCall += GenerateAtomFiles;
        }

        private static void GenerateAtomFiles()
        {
            var outputPath = "Packages/jp.notek.atomicsoup/UdonProgramSources/Atom/Typed/Generated/";
            Directory.CreateDirectory(outputPath);

            foreach (var type in Types.Concat(UdonDataContainerTypes))
            {
                SaveGeneratedFile(outputPath, GenerateAtomClass(type));
                SaveGeneratedFile(outputPath, GenerateWritableAtom(type));
                SaveGeneratedFile(outputPath, GenerateCalculatedAtom(type));
            }

            AssetDatabase.Refresh();
        }


        [MenuItem("AtomicSoup/Restore Atom Files")]
        private static void RestoreAtomFiles()
        {
            Debug.Log("Restoring Atom Files...");
            foreach (var filePath in AtomSourceBackup.instance.sourceByFileName.Keys)
            {
                Debug.Log($"Restoring {filePath}");
                File.WriteAllText(filePath, AtomSourceBackup.instance.sourceByFileName[filePath]);
            }
            AssetDatabase.Refresh();
        }

        private static void SaveGeneratedFile(string outputPath, (string className, string source) tuple)
        {
            var (className, source) = tuple;
            var filePath = Path.Combine(outputPath, $"{className}.cs");
            var backuped = BackupAtomSource(filePath);
            File.WriteAllText(filePath, source);
            if (!backuped)
                BackupAtomSource(filePath);
        }

        private static bool BackupAtomSource(string filePath)
        {
            try
            {
                var source = File.ReadAllText(filePath);
                var backup = AtomSourceBackup.instance.sourceByFileName;
                if (backup.ContainsKey(filePath))
                {
                    backup[filePath] = source;
                }
                else
                {
                    backup.Add(filePath, source);
                }
                AtomSourceBackup.instance.sourceByFileName = backup;

                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private static (string className, string source) GenerateAtomClass(string type)
        {
            var className = $"{char.ToUpper(type[0])}{type.Substring(1)}Atom";
            var sourceBuilder = new StringBuilder();
            //TODO: Value取得時に変更があれば反映させる
            sourceBuilder.AppendLine($@"
{GeneratedCodeAnnotationPrefix}
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class {className} : AtomSubscriberForAtom
    {{
        protected abstract {type} _Value {{ get; set; }}
        public {type} Value {{ get {{
            PublishIfUnchanged();
            return _Value;
        }} }}
        protected {type} _PreviousValue;
        public {type} PreviousValue {{ get {{
            PublishIfUnchanged();
            return _PreviousValue;
        }} }}
        
        protected {type} _NextValue;
        public {type} NextValue {{ get {{
            PublishIfUnchanged();
            return _NextValue;
        }} }}

        protected void SetNextValue({type} value)
        {{
            _NextValue = value;
            IsDirty = true;
            _Distributor.OnAtomChanged(this);
        }}

        public override void ReflectNextValue()
        {{
            _PreviousValue = _Value;
            _Value = _NextValue;
            IsDirty = false;
        }}

        void PublishIfUnchanged()
        {{
            _Distributor.PublishAtomValuesToContext();
        }}
    }}
}}");
            return (className, sourceBuilder.ToString());
        }

        private static (string className, string source) GenerateWritableAtom(string type)
        {
            var className = $"Writable{char.ToUpper(type[0])}{type.Substring(1)}Atom";
            var baseClassName = $"{char.ToUpper(type[0])}{type.Substring(1)}Atom";

            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine($@"
{GeneratedCodeAnnotationPrefix}
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{{
    public abstract class {className} : {baseClassName}
    {{
        public override void OnChange() {{ }}

        public void Set({type} value)
        {{
            SetNextValue(value);
        }}
    }}
}}");

            return (className, sourceBuilder.ToString());
        }

        private static (string className, string source) GenerateCalculatedAtom(string type)
        {
            var className = $"Calculated{char.ToUpper(type[0])}{type.Substring(1)}Atom";
            var baseClassName = $"{char.ToUpper(type[0])}{type.Substring(1)}Atom";


            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine($@"
{GeneratedCodeAnnotationPrefix}
using UnityEngine;
using VRC.SDKBase;
using VRC.SDK3.Data;

namespace JP.Notek.AtomicSoup
{{
    public abstract class {className} : {baseClassName}
    {{
        public abstract {type} Factory();
        
        public override void OnChange()
        {{
            SetNextValue(Factory());
        }}
    }}
}}");

            return (className, sourceBuilder.ToString());
        }

        private static string GeneratedCodeAnnotationPrefix = @"/*

    DO NOT MODIFY THIS FILE!

    THIS FILE IS GENERATED BY JP.Notek.AtomicSoup.Editor.AtomSourceGenerator.
*/";
    }
}
#endif