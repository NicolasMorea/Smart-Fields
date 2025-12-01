/// <summary>
///* Optional<T> can be used to make a field optional in the inspector.
///* you can still access the value even if it is not enabled, if you want to get a value only if it is enabled use TryGetValue
/// </summary>

using System;
using UnityEngine;

namespace Og.SmartFields
{
    [Serializable]
    public struct Optional<T>
    {
        [SerializeField] private bool enabled;
        [SerializeField] private T value;
        public readonly bool Enabled => enabled && value != null;
        public readonly T Value => value;
        public Optional(T value)
        {
            this.value = value;
            enabled = true;
        }
        public readonly T TryGetValue => enabled ? value : default;

        public readonly bool GetValue(out T val)
        {
            if (enabled)
            {
                val = value;
                return true;
            }
            val = default;
            return false;
        }
    }
}
