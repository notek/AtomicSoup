#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using UdonSharp;
using VRC.Udon;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEditor.Compilation;
using System.IO;

namespace JP.Notek.AtomicSoup.Editor
{
    [InitializeOnLoad]
    public class UdonSharpProgramAssetManager
    {
        static UdonSharpProgramAssetManager()
        {
            ConsistantUdonSharpProgramAssets();
        }

        public static void ConsistantUdonSharpProgramAssets()
        {
            if (EditorApplication.isPlaying)
                return;
            var managedScriptPaths = UdonSharpProgramAssetManagerSettings.instance.managedScriptPaths;
            var managedAssetPathByScriptPath = UdonSharpProgramAssetManagerSettings.instance.managedAssetPathByScriptPath;
            var allowPackageNames = UdonSharpProgramAssetManagerSettings.instance.allowPackageNames;

            var currentScripts = new HashSet<MonoScript>(
                AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets", "Packages/jp.notek.atomicsoup" }.Concat(allowPackageNames.Select(name => $"Packages/{name}")).ToArray())
                .Select(guid => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid)))
                .Where(script => script != null &&
                    script.GetClass()?.IsSubclassOf(typeof(UdonSharpBehaviour)) == true)
                .Where(script => script.GetClass()?.GetCustomAttributes(typeof(UdonSharpProgramAssetAttribute), true).Length > 0)
                );
            foreach (var script in currentScripts)
            {
                string scriptPath = GetScriptFullPath(script);
                if (managedAssetPathByScriptPath.ContainsKey(scriptPath))
                {
                    managedAssetPathByScriptPath[scriptPath] = GetAssetFullPath(script);
                }
                else
                {
                    managedAssetPathByScriptPath.Add(scriptPath, GetAssetFullPath(script));
                }
            }

            var deletedScriptPaths = new HashSet<string>(managedScriptPaths);
            deletedScriptPaths.ExceptWith(currentScripts.Select(script => GetScriptFullPath(script)));

            var addedScriptPaths = new HashSet<string>(currentScripts.Select(script => GetScriptFullPath(script)));
            addedScriptPaths.ExceptWith(managedScriptPaths);

            foreach (var scriptPath in addedScriptPaths)
            {
                if (!IsUdonProgramAssetExists(managedAssetPathByScriptPath[scriptPath]))
                {
                    CreateUdonProgramAsset(scriptPath, managedAssetPathByScriptPath[scriptPath]);
                }
                managedScriptPaths.Add(scriptPath);
            }
            foreach (var scriptPath in deletedScriptPaths)
            {
                if (!managedAssetPathByScriptPath.ContainsKey(scriptPath))
                {
                    continue;
                }
                string assetFullPath = managedAssetPathByScriptPath[scriptPath];
                DeleteAsset(assetFullPath);
                managedAssetPathByScriptPath.Remove(scriptPath);
                managedScriptPaths.Remove(scriptPath);
            }
            foreach (var asset in managedAssetPathByScriptPath)
            {
                ValidateAndFixUdonProgramAssets(asset.Value, asset.Key);
            }
            UdonSharpProgramAssetManagerSettings.instance.managedAssetPathByScriptPath = managedAssetPathByScriptPath;
            UdonSharpProgramAssetManagerSettings.instance.managedScriptPaths = managedScriptPaths;
        }

        private static string GetAssetFullPath(MonoScript sourceScript)
        {
            string sourcePath = AssetDatabase.GetAssetPath(sourceScript);
            string assetPath = System.IO.Path.GetDirectoryName(sourcePath);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
            string assetFullPath = $"{assetPath}/{assetName}.asset";
            return assetFullPath;
        }
        private static string GetScriptFullPath(MonoScript sourceScript)
        {
            return AssetDatabase.GetAssetPath(sourceScript);
        }

        private static bool IsUdonProgramAssetExists(string assetFullPath)
        {
            return AssetDatabase.LoadAssetAtPath<UdonSharpProgramAsset>(assetFullPath) != null;
        }

        private static void CreateUdonProgramAsset(string sourceScriptPath, string assetFullPath)
        {
            var programAsset = ScriptableObject.CreateInstance<UdonSharpProgramAsset>();
            AssetDatabase.CreateAsset(programAsset, assetFullPath);

            programAsset.sourceCsScript = AssetDatabase.LoadAssetAtPath<MonoScript>(sourceScriptPath);

            EditorUtility.SetDirty(programAsset);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created UdonProgramAsset at: {assetFullPath}");
            RepaintInspector(programAsset);
        }

        private static void ValidateAndFixUdonProgramAssets(string assetPath, string sourceScriptPath)
        {
            var asset = LoadAssetAtPathOrNull<UdonSharpProgramAsset>(assetPath);
            var script = LoadAssetAtPathOrNull<MonoScript>(sourceScriptPath);
            if (asset != null && script != null)
            {
                var assetSourceScript = asset.sourceCsScript;
                if (assetSourceScript != script)
                {
                    asset.sourceCsScript = script;
                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets();
                }
            }
            if (asset != null && script == null)
            {
                DeleteAsset(assetPath);
                AssetDatabase.SaveAssets();
            }
        }

        private static T LoadAssetAtPathOrNull<T>(string assetPath) where T : Object
        {
            try
            {
                return AssetDatabase.LoadAssetAtPath<T>(assetPath);
            }
            catch (FileNotFoundException)
            {
                Debug.LogError($"Failed to load asset at path: {assetPath}");
                return null;
            }
        }

        private static void DeleteAsset(string assetPath)
        {
            Debug.Log($"Deleting UdonProgramAsset: {assetPath}");
            AssetDatabase.DeleteAsset(assetPath);
        }

        private static void RepaintInspector(Object target)
        {
            var inspectorWindows = Resources.FindObjectsOfTypeAll<EditorWindow>()
                .Where(window => window.GetType().Name == "InspectorWindow");

            foreach (var inspectorWindow in inspectorWindows)
            {
                inspectorWindow.Repaint();
            }

            ActiveEditorTracker.sharedTracker?.ForceRebuild();

            var editor = ActiveEditorTracker.sharedTracker?.activeEditors
                .FirstOrDefault(e => e.target == target);
            if (editor != null)
            {
                editor.Repaint();
            }
        }
    }
}
#endif