using UnityEngine;
using UnityEngine.UI;

namespace Runner.Core.Utils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns new Color with Alpha set to a
        /// </summary>
        public static Color SetAlpha(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        /// <summary>
        /// Set Alpha of Image.Color
        /// </summary>
        public static void SetAlpha(this Graphic graphic, float a)
        {
            var color = graphic.color;
            color = new Color(color.r, color.g, color.b, a);
            graphic.color = color;
        }

        /// <summary>
        /// Set Alpha of Renderer.Color
        /// </summary>
        public static void SetAlpha(this SpriteRenderer renderer, float a)
        {
            var color = renderer.color;
            color = new Color(color.r, color.g, color.b, a);
            renderer.color = color;
        }
    }
}