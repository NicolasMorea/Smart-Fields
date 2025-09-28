/// <summary>
///* This attribute allows you to specify a enum that will determine whether the property is displayed or not
///* it needs a string reference to the enum field name, and indices in int format which is not ideal but it works
///* if there is a problem with the string reference, the editor will break but not the logic with the variables so no bugs
/// </summary>

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace Og.SmartFields
{
    [CustomPropertyDrawer(typeof(EnumDependentFieldAttribute))]
    public class EnumDependentFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumDependentFieldAttribute enumAttribute = (EnumDependentFieldAttribute)attribute;
            // SerializedProperty enumField;
            // enumField = property.serializedObject.FindProperty(enumAttribute.EnumFieldName);

            SerializedProperty enumField = property.serializedObject.FindProperty(enumAttribute.EnumFieldName);

            if (enumField == null)
            {
                // Try searching relative to the parent
                string parentPath = property.propertyPath.Substring(0, property.propertyPath.LastIndexOf('.'));
                enumField = property.serializedObject.FindProperty(parentPath + "." + enumAttribute.EnumFieldName);
            }

            if (enumField == null)
            {
                Debug.LogError($"Enum field not found in list element! Expected path: {enumAttribute.EnumFieldName}");
                EditorGUI.LabelField(position, label.text, "Invalid enum field reference.");
                return;
            }
            if (enumField.propertyType != SerializedPropertyType.Enum)
            {
                EditorGUI.LabelField(position, label.text, "Invalid SerializedPropertyType");
                return;
            }

            bool isFlagsEnum = Attribute.IsDefined(enumAttribute.EnumType, typeof(FlagsAttribute));

            bool shouldShow = false;

            int currentEnumValue = enumField.intValue;

            if (isFlagsEnum)
            {
                // For [Flags] enums: Use bitwise logic to check if any specified flags are set
                foreach (int valueIndex in enumAttribute.EnumValueIndices)
                {
                    int flagValue = 1 << valueIndex; // Convert index to bitwise flag
                    if ((currentEnumValue & flagValue) != 0) // Check if flag is set
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }
            else
            {
                // For standard enums: Check direct index match
                foreach (int valueIndex in enumAttribute.EnumValueIndices)
                {
                    if (currentEnumValue == valueIndex) // Match specific value directly
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }

            if (shouldShow)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }


            // EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            EnumDependentFieldAttribute enumAttribute = (EnumDependentFieldAttribute)attribute;

            // find property even in nested properties
            // string enumFieldPath = property.propertyPath.Replace(property.name, enumAttribute.EnumFieldName);
            // SerializedProperty enumField = property.serializedObject.FindProperty(enumFieldPath);
            SerializedProperty enumField;
            enumField = property.serializedObject.FindProperty(enumAttribute.EnumFieldName);


            if (enumField == null || enumField.propertyType != SerializedPropertyType.Enum)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            // Get the current enum value
            int currentEnumValue = enumField.intValue;

            // Determine if the enum type is a [Flags] enum
            bool isFlagsEnum = Attribute.IsDefined(enumAttribute.EnumType, typeof(FlagsAttribute));

            bool shouldShow = false;

            if (isFlagsEnum)
            {
                // For [Flags] enums: Use bitwise logic to check if any specified flags are set
                foreach (int valueIndex in enumAttribute.EnumValueIndices)
                {
                    int flagValue = 1 << valueIndex; // Convert index to bitwise flag
                    if ((currentEnumValue & flagValue) != 0) // Check if flag is set
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }
            else
            {
                // For standard enums: Check direct index match
                foreach (int valueIndex in enumAttribute.EnumValueIndices)
                {
                    if (currentEnumValue == valueIndex) // Match specific value directly
                    {
                        shouldShow = true;
                        break;
                    }
                }
            }
            return shouldShow ? EditorGUI.GetPropertyHeight(property, true) : 0;
        }

    }
}
#endif
