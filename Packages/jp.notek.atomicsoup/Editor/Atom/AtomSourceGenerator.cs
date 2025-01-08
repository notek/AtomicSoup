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
        static AtomSourceGenerator()
        {
            EditorApplication.delayCall += GenerateAtomFiles;
        }

        private static void GenerateAtomFiles()
        {
            var outputPath = "Packages/jp.notek.atomicsoup/UdonProgramSources/Atom/Typed/Generated/";
            Directory.CreateDirectory(outputPath);

            foreach (var type in AtomTypes.Syncable)
            {
                SaveGeneratedFile(outputPath, GenerateAtomClass(type));
                SaveGeneratedFile(outputPath, GenerateWritableAtom(type));
                SaveGeneratedFile(outputPath, GenerateCalculatedAtom(type));
                SaveGeneratedFile(outputPath, GenerateSyncableAtom(type));
            }
            foreach (var type in AtomTypes.UdonDataContainer)
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
            if ({(type == "Color32" ? "_NextValue.Equals(value)" : "_NextValue == value")})
                return;
            _NextValue = value;
            IsDirty = true;
        }}

        public override void ReflectNextValue()
        {{
            _PreviousValue = _Value;
            _Value = _NextValue;
            IsDirty = false;
        }}

        void PublishIfUnchanged()
        {{
            _Distributor.PublishPrimary();
        }}
    }}
}}");
            return (className, sourceBuilder.ToString());
        }

        private static (string className, string source) GenerateWritableAtom(string type)
        {
            var baseClassName = AtomTypes.ToAtomType(type);
            var className = $"Writable{baseClassName}";

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
        public override bool IsInputNode {{ get {{ return true; }} }}
        public override void OnChange() {{ }}

        public void Set({type} value)
        {{
            SetNextValue(value);
            _Distributor.OnInputNodeChanged(this);
        }}
    }}
}}");

            return (className, sourceBuilder.ToString());
        }

        private static (string className, string source) GenerateCalculatedAtom(string type)
        {
            var baseClassName = AtomTypes.ToAtomType(type);
            var className = $"Calculated{baseClassName}";


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

        private static (string className, string source) GenerateSyncableAtom(string type)
        {
            var baseClassName = AtomTypes.ToAtomType(type);
            var className = $"Syncable{baseClassName}";


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
        public override bool IsInputNode {{ get {{ return true; }} }}
        public override void OnChange() {{ }}

        public void Set({type} value)
        {{
            SetNextValue(value);
            _Distributor.OnInputNodeChanged(this);
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