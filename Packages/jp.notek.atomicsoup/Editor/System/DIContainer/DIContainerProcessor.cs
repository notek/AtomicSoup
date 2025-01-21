using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UdonSharp;
using System.Collections.Generic;
using VRC.Core.Source.Config.Interfaces;
using UdonSharpEditor;
namespace JP.Notek.AtomicSoup.Editor
{
    [InitializeOnLoad]
    public class DIContainerProcessor
    {
        static DIContainerProcessor()
        {
            EditorApplication.hierarchyChanged += Process;
            Process();
        }

        private static void Process()
        {
            if(EditorApplication.isPlaying)
                return;
            var behaviours = Object.FindObjectsOfType<UdonSharpBehaviour>();
            foreach (var behaviour in behaviours)
            {
                ProcessDependencies(behaviour);
            }

            var behaviourInstanceIds = new HashSet<int>();
            foreach (var behaviour in behaviours)
            {
                if (behaviour.GetType().GetCustomAttributes(typeof(DIProvideAttribute), true).FirstOrDefault() as DIProvideAttribute != null)
                    behaviourInstanceIds.Add(behaviour.GetInstanceID());
            }
            var managedComponentInstanceIdsKeysByProviderComponentInstanceId = DIContainerSettings.instance.managedComponentInstanceIdsKeysByProviderComponentInstanceId;
            var managedComponentInstanceIds = DIContainerSettings.instance.managedComponentInstanceIds;
            var removedProviderComponentInstanceIds = managedComponentInstanceIdsKeysByProviderComponentInstanceId.Keys.Except(behaviourInstanceIds);
            foreach (var providerComponentInstanceId in removedProviderComponentInstanceIds)
            {
                foreach (var key in managedComponentInstanceIdsKeysByProviderComponentInstanceId[providerComponentInstanceId])
                {
                    var removeComponent = EditorUtility.InstanceIDToObject(managedComponentInstanceIds[key]);
                    if (removeComponent != null)
                        Object.DestroyImmediate(removeComponent);
                    managedComponentInstanceIds.Remove(key);
                }
                managedComponentInstanceIdsKeysByProviderComponentInstanceId.Remove(providerComponentInstanceId);
            }
            DIContainerSettings.instance.managedComponentInstanceIdsKeysByProviderComponentInstanceId = managedComponentInstanceIdsKeysByProviderComponentInstanceId;
            DIContainerSettings.instance.managedComponentInstanceIds = managedComponentInstanceIds;
        }

        private static void ProcessDependencies(UdonSharpBehaviour behaviour)
        {
            var provideAttributes = behaviour.GetType().GetCustomAttributes(typeof(DIProvideAttribute), true).Cast<DIProvideAttribute>();
            foreach (var provideAttribute in provideAttributes)
            {
                ProcessProvide(behaviour, provideAttribute);
            }

            var fields = behaviour.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var injectAttribute = field.GetCustomAttributes(typeof(DIInjectAttribute), true).FirstOrDefault() as DIInjectAttribute;
                if (injectAttribute != null)
                {
                    ProcessInject(behaviour, field, injectAttribute);
                }
            }
        }

        private static void ProcessProvide(UdonSharpBehaviour behabiour, DIProvideAttribute attribute)
        {
            var managedComponentInstanceIds = DIContainerSettings.instance.managedComponentInstanceIds;
            if (managedComponentInstanceIds.TryGetValue(GetManagedComponentInstanceIdsKey(attribute.ComponentType, attribute.Id), out var instanceId))
            {
                var components = behabiour.gameObject.GetComponents(attribute.ComponentType);
                var componentFound = components.FirstOrDefault(component => component.GetInstanceID() == instanceId);
                if (componentFound != null)
                    return;
            }
            var newComponent = behabiour.gameObject.AddComponent(attribute.ComponentType);
            var key = GetManagedComponentInstanceIdsKey(attribute.ComponentType, attribute.Id);

            if (managedComponentInstanceIds.ContainsKey(key))
            {
                managedComponentInstanceIds[key] = newComponent.GetInstanceID();
            }
            else
            {
                managedComponentInstanceIds.Add(key, newComponent.GetInstanceID());
            }
            DIContainerSettings.instance.managedComponentInstanceIds = managedComponentInstanceIds;
        }

        private static string GetManagedComponentInstanceIdsKey(System.Type componentType, string id)
        {
            return id != null ? $"{componentType.Name}+{id}" : $"{componentType.Name}";
        }

        private static void ProcessInject(UdonSharpBehaviour behaviour, FieldInfo field, DIInjectAttribute attribute)
        {
            var componentType = attribute.Type ?? field.FieldType;
            DIContainerSettings.instance.managedComponentInstanceIds.TryGetValue(GetManagedComponentInstanceIdsKey(componentType, attribute.Id), out var instanceId);
            var component = FindComponentInHierarchy(behaviour.gameObject, componentType, attribute.Id != null ? instanceId : null);

            if (component != null)
            {
                field.SetValue(behaviour, component);
                UdonSharpEditorUtility.CopyProxyToUdon(behaviour);
            }
            else
            {
                if (attribute.Id != null)
                    Debug.LogError($"[AutoDI] Required component {componentType.Name}:{attribute.Id} not provided in hierarchy of {behaviour.gameObject.name}");
                else
                    Debug.LogError($"[AutoDI] Required component {componentType.Name} not provided in hierarchy of {behaviour.gameObject.name}");
            }
        }

        private static Component FindComponentInHierarchy(GameObject gameObject, System.Type componentType, int? instanceId)
        {
            var components = gameObject.GetComponents(componentType);
            if (components.Length > 0 && instanceId == null)
            {
                return components[0];
            }
            else if (components.Length > 0 && instanceId != null)
            {
                var componentMatched = components.First(component => component.GetInstanceID() == instanceId);
                if (componentMatched != null)
                    return componentMatched;
            }


            if (gameObject.transform.parent != null)
            {
                return FindComponentInHierarchy(gameObject.transform.parent.gameObject, componentType, instanceId);
            }

            return null;
        }
    }
}