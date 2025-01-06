#if !COMPILER_UDONSHARP && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UdonSharp;

namespace JP.Notek.AtomicSoup.Editor
{

    [CustomPropertyDrawer(typeof(UdonSharpBehaviour), true)]
    public class DIPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (fieldInfo != null && fieldInfo.GetCustomAttributes(typeof(DIInjectAttribute), true).Length > 0)
            {
                OverrideGUI(position, property, label);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (fieldInfo != null && fieldInfo.GetCustomAttributes(typeof(DIInjectAttribute), true).Length > 0 && property.objectReferenceValue == null)
            {
                return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight;
            }
            return base.GetPropertyHeight(property, label);
        }
        public void OverrideGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.DisabledGroupScope(true))
            {
                Color originalBackgroundColor = GUI.backgroundColor;

                var style = new GUIStyle(EditorStyles.label);
                style.richText = true;

                string statusText = property.objectReferenceValue == null
                    ? $"<color=#FFFFFF>{label.text}</color> <color=#FF0000>[DI not provided]</color>"
                    : $"<color=#FFFFFF>{label.text}</color> <color=#00FF00>[DI provided]</color>";

                var richLabel = new GUIContent(statusText, label.image, label.tooltip);

                var labelRect = position;
                labelRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(labelRect, richLabel, style);

                var fieldRect = position;
                fieldRect.height = EditorGUIUtility.singleLineHeight;
                fieldRect.x += EditorGUIUtility.labelWidth;
                fieldRect.width -= EditorGUIUtility.labelWidth;

                if (property.objectReferenceValue == null)
                {
                    GUI.backgroundColor = new Color(1f, 0.0f, 0.0f);
                }
                else
                {
                    GUI.backgroundColor = new Color(0.0f, 1f, 0.0f);
                }

                EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);

                GUI.backgroundColor = originalBackgroundColor;

                if (property.objectReferenceValue == null)
                {
                    var helpBoxRect = position;
                    helpBoxRect.y += EditorGUI.GetPropertyHeight(property, label, true);
                    helpBoxRect.height = EditorGUIUtility.singleLineHeight;
                    EditorGUI.HelpBox(helpBoxRect, "This component will be automatically injected. Please ensure the target Provider exists in the scene.", MessageType.Info);
                }
            }
        }
    }
}
#endif