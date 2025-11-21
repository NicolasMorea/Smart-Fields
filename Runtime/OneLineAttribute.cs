/// <summary>
///* Draws a whole class or struct in one Line for cleaner inspector view.
/// </summary>

using UnityEngine;

namespace Og.SmartFields
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true)]
    public class OneLinePropertyAttribute : PropertyAttribute { }
}