
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace JP.Notek.AtomicSoup.Editor
{
    [FilePath("ProjectSettings/JP.Notek.AtomicSoup.Editor/DIContainerSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class DIContainerSettings : ScriptableSingleton<DIContainerSettings>
    {
        [SerializeField]
        private List<string> _managedComponentInstanceIdsKeys = new List<string>();

        [SerializeField]
        private List<int> _managedComponentInstanceIdsValues = new List<int>();

        [SerializeField]
        private List<int> _managedComponentInstanceIdsKeysByProviderComponentInstanceIdKeys = new List<int>();

        [SerializeField]
        private List<string> _managedComponentInstanceIdsKeysByProviderComponentInstanceIdValues = new List<string>();
        public Dictionary<int, string[]> managedComponentInstanceIdsKeysByProviderComponentInstanceId
        {
            get
            {
                var dict = new Dictionary<int, string[]>();
                for (int i = 0; i < _managedComponentInstanceIdsKeysByProviderComponentInstanceIdKeys.Count; i++)
                {
                    dict[_managedComponentInstanceIdsKeysByProviderComponentInstanceIdKeys[i]] = _managedComponentInstanceIdsKeysByProviderComponentInstanceIdValues[i].Split('&');
                }
                return dict;
            }
            set
            {
                _managedComponentInstanceIdsKeysByProviderComponentInstanceIdKeys = new List<int>(value.Keys);
                _managedComponentInstanceIdsKeysByProviderComponentInstanceIdValues = new List<string>(value.Values.Select(v => String.Join("&", v)));
                Save();
            }
        }

        public Dictionary<string, int> managedComponentInstanceIds
        {
            get
            {
                var dict = new Dictionary<string, int>();
                for (int i = 0; i < _managedComponentInstanceIdsKeys.Count; i++)
                {
                    dict[_managedComponentInstanceIdsKeys[i]] = _managedComponentInstanceIdsValues[i];
                }
                return dict;
            }
            set
            {
                _managedComponentInstanceIdsKeys = new List<string>(value.Keys);
                _managedComponentInstanceIdsValues = new List<int>(value.Values);
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