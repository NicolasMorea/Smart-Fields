/// <summary>
///* The GUI of the Optional<T> struct, a checkbox to enable/disable the field
///* We grey out the field when disabled
/// </summary>

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Og.SmartFields
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("value"));
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var enabled = property.FindPropertyRelative("enabled");
            var value = property.FindPropertyRelative("value");

            // Save indent level
            int oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Reserve space for toggle (square button)
            float toggleSize = EditorGUIUtility.singleLineHeight;
            Rect valueRect = new Rect(position.x, position.y, position.width - toggleSize - 2, position.height);
            Rect toggleRect = new Rect(position.x + position.width - toggleSize, position.y, toggleSize, toggleSize);

            // Draw value with label
            EditorGUI.BeginDisabledGroup(!enabled.boolValue);
            EditorGUI.PropertyField(valueRect, value, label, true);
            EditorGUI.EndDisabledGroup();

            // Draw toggle
            EditorGUI.PropertyField(toggleRect, enabled, GUIContent.none);

            // Restore indent
            EditorGUI.indentLevel = oldIndent;
        }
    }

    
}
#endif
