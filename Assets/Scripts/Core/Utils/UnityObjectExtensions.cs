using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Runner.Core.Utils
{
    public static class UnityObjectExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryEnable(this Behaviour behaviour, bool enabled)
        {
            if (behaviour == null)
            {
                return false;
            }
            
            behaviour.enabled = enabled;

            return enabled;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetActive(this Component component, bool active)
        {
            return component != null && component.gameObject.TrySetActive(active);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TrySetActive(this GameObject gameObject, bool active)
        {
            if (gameObject == null)
            {
                return false;
            }

            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }

            return active;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDestroyed(this Component component)
        {
            return component == null;
        }

        [Conditional("UNITY_EDITOR")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetName<T>(this T instance, string name) where T : Object
        {
            instance.name = name;
        }
    }
}
