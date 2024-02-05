using System.Runtime.CompilerServices;
using UnityEngine;

namespace Runner.Core.Utils
{
    public static class CanvasGroupExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsVisible(this CanvasGroup canvasGroup)
        {
            if (canvasGroup == null)
                return false;

            return canvasGroup.alpha > 0.0f;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetVisibility(this CanvasGroup canvasGroup, bool value)
        {
            if (canvasGroup == null)
                return;

            canvasGroup.alpha = value == true ? 1.0f : 0.0f;
        }
    }
}