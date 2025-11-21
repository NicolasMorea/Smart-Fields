/// <summary>
///* Attribute to pick a type for a field in the Unity Inspector.
//* the nested field are then displayed.
//! must be used in conjunction with [SerializeReference] to work (to access polymorphic types)
/// </summary>

#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace Og.SmartFields
{
    [CustomPropertyDrawer(typeof(TypePickAttribute))]
    public class TypePickPropertyDrawer : PropertyDrawer
    {
        private GenericSearchTree searchWindowProvider;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            // Debug.Log("TypePickPropertyDrawer OnGUI called for " + label.text);

            bool hasSerializeReference = fieldInfo.GetCustomAttribute<SerializeReference>() != null;
            if (!hasSerializeReference)
            {
                DrawMissingReferenceWarning(position, label, $"{label.text} requires [SerializeReference]");
                return;
            }

            Debug.Assert(fieldInfo.FieldType.IsClass, $"{label.text} must be a reference type (class)");
            
            // Ensure property is valid
            if (property == null)
            {
                DrawMissingReferenceWarning(position, label, $"Property {label.text} is null.");
                return;
            }

            // Build button label: "<Label>: <Type or 'Select Type'>"
            string currentTypeName = property.managedReferenceValue != null
                ? property.managedReferenceValue.GetType().Name
                : "Select Type";
            string buttonText = $"{label.text}: {currentTypeName}";

            Rect buttonRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect fieldRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + 2,
                position.width,
                position.height - (EditorGUIUtility.singleLineHeight + 2)
            );


            // Draw single button
            if (GUI.Button(buttonRect, buttonText, EditorStyles.popup))
            {
                ShowSearchWindow(property);
            }

            // Draw nested fields if we have a valid instance
            if (property.managedReferenceValue != null)
            {
                EditorGUI.indentLevel = 0;
                EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool hasSerializeReference = fieldInfo.GetCustomAttribute<SerializeReference>() != null;

            if (!hasSerializeReference)
            {
                // Reserve a full line for the purple message
                return EditorGUIUtility.singleLineHeight * 2f;
            }

            float height = EditorGUIUtility.singleLineHeight + 2;
            if (property.managedReferenceValue != null)
                height += EditorGUI.GetPropertyHeight(property, label, true);

            return height;
        }
        
        private void ShowSearchWindow(SerializedProperty property)
        {
            if (searchWindowProvider == null)
            {
                searchWindowProvider = ScriptableObject.CreateInstance<GenericSearchTree>();
                searchWindowProvider.InitCategories(GetMainType(), GetMainCategories());
                searchWindowProvider.Initialize(type =>
                {
                    SetType(type, property);
                });
            }

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)),
                            searchWindowProvider);
        }

        private void DrawMissingReferenceWarning(Rect position, GUIContent label, string msg)
        {
            GUIStyle style = new GUIStyle(EditorStyles.helpBox)
            {
                normal = { textColor = new Color(0.6f, 0.4f, 0.9f) }, // purple because why not
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };
            EditorGUI.LabelField(position, msg, style);
        }

        private void SetType(Type type, SerializedProperty property)
        {
            property.managedReferenceValue = Activator.CreateInstance(type);
            property.serializedObject.ApplyModifiedProperties();
        }

        private Type GetMainType()
        {
            return fieldInfo.FieldType;
        }

        private List<Type> GetMainCategories()
        {
            return new List<Type>();
        }
    }
}
#endif