/// <summary>
///* Attribute to pick a type for a field in the Unity Inspector.
//* the nested field are then displayed.
//! must be used in conjunction with [SerializeReference] to work (to access polymorphic types)
/// </summary>

using UnityEngine;
using System;

namespace Og.SmartFields
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class TypePickAttribute : PropertyAttribute { }
}