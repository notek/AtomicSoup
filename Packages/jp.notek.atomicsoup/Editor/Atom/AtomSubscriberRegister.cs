#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using UdonSharp;
using VRC.Udon;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UdonSharpEditor;
using VRC.Udon.Common.Interfaces;
using UnityEditor.Compilation;

namespace JP.Notek.AtomicSoup.Editor
{
    [InitializeOnLoad]
    public class AtomSubscriberRegister
    {
        static AtomSubscriberRegister()
        {
            EditorApplication.hierarchyChanged += Register;
            Register();
        }

        public static void Register()
        {
            if(EditorApplication.isPlaying)
                return;
            var distributors = Object.FindObjectsOfType<AtomDistributor>();
            var subscribersForView = Object.FindObjectsOfType<AtomSubscriberForView>();
            var subscribersForAtom = Object.FindObjectsOfType<AtomSubscriberForAtom>();
            var subscribersForSync = Object.FindObjectsOfType<AtomSubscriberForSync>();
            foreach (var distributor in distributors)
            {
                distributor.ClearSubscribers();
            }
            foreach (var subscriberForView in subscribersForView)
            {
                var subscribeAtoms = subscriberForView.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttributes(typeof(SubscribeAtomAttribute), true).Length > 0)
                    .Select(field => field.GetValue(subscriberForView) as AtomSubscriber)
                    .Where(atom => atom != null)
                    .ToArray();
                if (subscribeAtoms.Length > 0)
                    foreach (var distributor in distributors)
                    {
                        distributor.SubscribeOnChangeForView(subscriberForView, subscribeAtoms);
                        UdonSharpEditorUtility.CopyProxyToUdon(distributor);
                    }
            }
            foreach (var subscriberForAtom in subscribersForAtom)
            {
                var subscribeAtoms = subscriberForAtom.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttributes(typeof(SubscribeAtomAttribute), true).Length > 0)
                    .Select(field => field.GetValue(subscriberForAtom) as AtomSubscriber)
                    .Where(atom => atom != null)
                    .ToArray();
                if (subscribeAtoms.Length > 0)
                    foreach (var distributor in distributors)
                    {
                        distributor.SubscribeOnChangeForContext(subscriberForAtom, subscribeAtoms);
                        UdonSharpEditorUtility.CopyProxyToUdon(distributor);
                    }
            }
            foreach (var subscriberForSync in subscribersForSync)
            {
                var subscribeAtoms = subscriberForSync.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                    .Where(field => field.GetCustomAttributes(typeof(SubscribeAtomAttribute), true).Length > 0)
                    .Select(field => field.GetValue(subscriberForSync) as AtomSubscriber)
                    .Where(atom => atom != null)
                    .ToArray();
                if (subscribeAtoms.Length > 0)
                    foreach (var distributor in distributors)
                    {
                        distributor.SubscribeOnChangeForSync(subscriberForSync, subscribeAtoms);
                        UdonSharpEditorUtility.CopyProxyToUdon(distributor);
                    }
            }

            foreach (var distributor in distributors)
            {
                distributor.FinalizePreprocess();
            }
        }
    }
}
#endif