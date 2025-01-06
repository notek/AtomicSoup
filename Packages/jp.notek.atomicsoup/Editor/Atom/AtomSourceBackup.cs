
#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace JP.Notek.AtomicSoup.Editor
{
    [FilePath("ProjectSettings/JP.Notek.AtomicSoup.Editor/AtomSourceBackup.asset", FilePathAttribute.Location.ProjectFolder)]
    public class AtomSourceBackup : ScriptableSingleton<AtomSourceBackup>
    {
        [SerializeField]
        private List<string> _sourceByFileNameKeys = new List<string>();

        [SerializeField]
        private List<string> _sourceByFileNameValues = new List<string>();
        public Dictionary<string, string> sourceByFileName
        {
            get
            {
                var dict = new Dictionary<string, string>();
                for (int i = 0; i < _sourceByFileNameKeys.Count; i++)
                {
                    dict[_sourceByFileNameKeys[i]] = _sourceByFileNameValues[i];
                }
                return dict;
            }
            set
            {
                _sourceByFileNameKeys = new List<string>(value.Keys);
                _sourceByFileNameValues = new List<string>(value.Values);
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