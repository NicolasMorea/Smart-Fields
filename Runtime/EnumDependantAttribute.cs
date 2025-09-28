/// <summary>
///* This attribute allows you to specify a enum that will determine whether the property is displayed or not
///* it needs a string reference to the enum field name, and indices in int format which is not ideal but it works
///* if there is a problem with the string reference, the editor will break but not the logic with the variables so no bugs
/// </summary>

using System;
using UnityEngine;

namespace Og.SmartFields
{
    public class EnumDependentFieldAttribute : PropertyAttribute
    {
        public string EnumFieldName { get; }
        public Type EnumType { get; }
        public int[] EnumValueIndices { get; }

        public EnumDependentFieldAttribute(string enumFieldName, Type enumType, params int[] enumValueIndices)
        {
            EnumFieldName = enumFieldName;
            EnumType = enumType;
            EnumValueIndices = enumValueIndices;
        }
    }
}
