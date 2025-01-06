
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace JP.Notek.AtomicSoup.Editor
{
    [FilePath("ProjectSettings/JP.Notek.AtomicSoup.Editor/UdonSharpProgramAssetManagerSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UdonSharpProgramAssetManagerSettings : ScriptableSingleton<UdonSharpProgramAssetManagerSettings>
    {
        [SerializeField]
        private List<MonoScript> _managedScriptsList = new List<MonoScript>();

        [SerializeField]
        private List<string> _managedAssetsKeys = new List<string>();

        [SerializeField]
        private List<MonoScript> _managedAssetsValues = new List<MonoScript>();

        public HashSet<MonoScript> managedScripts
        {
            get => new HashSet<MonoScript>(_managedScriptsList);
            set
            {
                _managedScriptsList = new List<MonoScript>(value);
                Save();
            }
        }

        public Dictionary<string, MonoScript> managedAssets
        {
            get
            {
                var dict = new Dictionary<string, MonoScript>();
                for (int i = 0; i < _managedAssetsKeys.Count; i++)
                {
                    dict[_managedAssetsKeys[i]] = _managedAssetsValues[i];
                }
                return dict;
            }
            set
            {
                _managedAssetsKeys = new List<string>(value.Keys);
                _managedAssetsValues = new List<MonoScript>(value.Values);
                Save();
            }
        }

        private void Save()
        {
            Save(true);
        }
    }
}
#endif