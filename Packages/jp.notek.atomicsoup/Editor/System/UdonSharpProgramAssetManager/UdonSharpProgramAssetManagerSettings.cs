
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
        private List<string> _allowPackageNames = new List<string>();

        [SerializeField]
        private List<string> _managedScriptsList = new List<string>();

        [SerializeField]
        private List<string> _managedAssetsKeys = new List<string>();

        [SerializeField]
        private List<string> _managedAssetsValues = new List<string>();

        public HashSet<string> allowPackageNames
        {
            get => new HashSet<string>(_allowPackageNames);
            set
            {
                _allowPackageNames = new List<string>(value);
                Save();
            }
        }

        public HashSet<string> managedScriptPaths
        {
            get => new HashSet<string>(_managedScriptsList);
            set
            {
                _managedScriptsList = new List<string>(value);
                Save();
            }
        }

        public Dictionary<string, string> managedAssetPathByScriptPath
        {
            get
            {
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < _managedAssetsKeys.Count; i++)
                {
                    dict[_managedAssetsKeys[i]] = _managedAssetsValues[i];
                }
                return dict;
            }
            set
            {
                _managedAssetsKeys = new List<string>(value.Keys);
                _managedAssetsValues = new List<string>(value.Values);
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