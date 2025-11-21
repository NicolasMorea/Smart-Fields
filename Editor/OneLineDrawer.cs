/// <summary>
///* The drawer retrieves all fields and draws them in one line.
///* They all take the same width except booleans which have a fixed width. (check boxes)
/// </summary>

using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

namespace Og.SmartFields
{
    [CustomPropertyDrawer(typeof(OneLinePropertyAttribute))]
    public class OneLinePropertyDrawer : PropertyDrawer
    {
        private const float FieldSpacing = 4f;
        private const float BoolWidth = 18f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.isArray && property.propertyType != SerializedPropertyType.String) DrawArray(position, property, label);
            else DrawInline(position, property, label);

            EditorGUI.EndProperty();
        }

        private void DrawArray(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        private void DrawInline(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.BeginChangeCheck();

            int indent = EditorGUI.indentLevel;
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;

            var iterator = property.Copy();
            var end = iterator.GetEndProperty();

            var children = new System.Collections.Generic.List<SerializedProperty>();
            while (iterator.NextVisible(true) && !SerializedProperty.EqualContents(iterator, end))
            {
                children.Add(iterator.Copy());
            }

            // Count boolean and non-boolean fields
            int boolCount = 0;
            int nonBoolCount = 0;
            foreach (var child in children)
            {
                if (child.propertyType == SerializedPropertyType.Boolean)
                    boolCount++;
                else
                    nonBoolCount++;
            }

            // Compute total width available and spacing
            float totalSpacing = FieldSpacing * Mathf.Max(0, children.Count - 1);
            float remainingWidth = position.width - totalSpacing - (boolCount * BoolWidth);
            float normalFieldWidth = nonBoolCount > 0 ? remainingWidth / nonBoolCount : 0f;

            // Draw all children
            Rect fieldRect = new Rect(position.x, position.y, 0, EditorGUIUtility.singleLineHeight);

            foreach (var child in children)
            {
                float width = (child.propertyType == SerializedPropertyType.Boolean) ? BoolWidth : normalFieldWidth;
                fieldRect.width = width;

                EditorGUI.PropertyField(fieldRect, child, GUIContent.none, true);

                fieldRect.x += width + FieldSpacing;
            }

            EditorGUI.indentLevel = indent;
            EditorGUIUtility.labelWidth = labelWidth;

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isArray && property.propertyType != SerializedPropertyType.String) return EditorGUI.GetPropertyHeight(property, label, true);

            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif