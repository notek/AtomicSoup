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

namespace JP.Notek.AtomicSoup.Editor
{
    [InitializeOnLoad]
    public class UdonSharpProgramAssetManager
    {
        static UdonSharpProgramAssetManager()
        {
            EditorApplication.hierarchyChanged += ConsistantUdonSharpProgramAssets;
            CompilationPipeline.compilationFinished += (object sender) => ConsistantUdonSharpProgramAssets();
        }

        public static void ConsistantUdonSharpProgramAssets()
        {
            if (EditorApplication.isPlaying)
                return;
            var managedScripts = UdonSharpProgramAssetManagerSettings.instance.managedScripts;
            var managedAssets = UdonSharpProgramAssetManagerSettings.instance.managedAssets;

            var currentScripts = new HashSet<MonoScript>(
                Object.FindObjectsOfType<UdonSharpBehaviour>()
                .Select(script => (MonoScript.FromMonoBehaviour(script), script.GetType()))
                .Where(result => result.Item2.GetCustomAttributes(typeof(UdonSharpProgramAssetAttribute), true).Length > 0)
                .Select(result => result.Item1)
                );
            var deletedScripts = new HashSet<MonoScript>(managedScripts);
            deletedScripts.ExceptWith(currentScripts);
            var addedScripts = new HashSet<MonoScript>(currentScripts);
            addedScripts.ExceptWith(managedScripts);

            foreach (var script in addedScripts)
            {
                string assetFullPath = GetAssetFullPath(script);
                if (!IsUdonProgramAssetExists(assetFullPath))
                {
                    CreateUdonProgramAsset(script, assetFullPath);
                }
                managedScripts.Add(script);
                managedAssets.Add(assetFullPath, script);
            }
            foreach (var script in deletedScripts)
            {
                //TODO: スクリプトが削除された場合nullになるので、パスを保存するようにする。
                if (script == null)
                {
                    continue;
                }
                string assetFullPath = GetAssetFullPath(script);
                DeleteAsset(assetFullPath);
                managedScripts.Remove(script);
                managedAssets.Remove(assetFullPath);
            }
            foreach (var asset in managedAssets)
            {
                ValidateAndFixUdonProgramAssets(asset.Key, asset.Value);
            }
            UdonSharpProgramAssetManagerSettings.instance.managedScripts = managedScripts;
            UdonSharpProgramAssetManagerSettings.instance.managedAssets = managedAssets;
        }

        private static string GetAssetFullPath(MonoScript sourceScript)
        {
            string sourcePath = AssetDatabase.GetAssetPath(sourceScript);
            string assetPath = System.IO.Path.GetDirectoryName(sourcePath);
            string assetName = System.IO.Path.GetFileNameWithoutExtension(sourcePath);
            string assetFullPath = $"{assetPath}/{assetName}.asset";
            return assetFullPath;
        }

        private static bool IsUdonProgramAssetExists(string assetFullPath)
        {
            return AssetDatabase.LoadAssetAtPath<UdonSharpProgramAsset>(assetFullPath) != null;
        }

        private static void CreateUdonProgramAsset(MonoScript sourceScript, string assetFullPath)
        {
            var programAsset = ScriptableObject.CreateInstance<UdonSharpProgramAsset>();
            AssetDatabase.CreateAsset(programAsset, assetFullPath);

            programAsset.sourceCsScript = AssetDatabase.LoadAssetAtPath<MonoScript>(
                AssetDatabase.GetAssetPath(sourceScript));

            EditorUtility.SetDirty(programAsset);
            AssetDatabase.SaveAssets();

            Debug.Log($"Created UdonProgramAsset at: {assetFullPath}");
            RepaintInspector(programAsset);
        }

        private static void ValidateAndFixUdonProgramAssets(string assetPath, MonoScript sourceScript)
        {
            var programAsset = AssetDatabase.LoadAssetAtPath<UdonSharpProgramAsset>(assetPath);
            if (programAsset == null)
            {
                return;
            }
            var assetSourceScript = programAsset.sourceCsScript;
            if (assetSourceScript != sourceScript)
            {
                programAsset.sourceCsScript = sourceScript;
                EditorUtility.SetDirty(programAsset);
                AssetDatabase.SaveAssets();
            }
        }

        private static void DeleteAsset(string assetPath)
        {
            Debug.Log($"Deleting unused UdonProgramAsset: {assetPath}");
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

            ActiveEditorTracker.sharedTracker.ForceRebuild();

            var editor = ActiveEditorTracker.sharedTracker.activeEditors
                .FirstOrDefault(e => e.target == target);
            if (editor != null)
            {
                editor.Repaint();
            }
        }
    }
}
#endif