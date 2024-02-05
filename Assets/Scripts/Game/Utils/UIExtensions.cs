using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Runner.Game.Utils
{
    public static class UIExtensions
    {
        public static readonly Vector2 DefaultAnchor = Vector2.one * 0.5f;

        private const string StartTag = "<";
        private const string EndTag = ">";
        private const string StartSizeTag = "<size=";
        private const string EndSizeTag = "</size>";
        private const string StartColorTag = "<color=";
        private const string EndColorTag = "</size>";
        private const string HashSymbol = "#";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AsRichText(this Color color, StringBuilder sb, string str)
        {
            sb.StartColor(ColorUtility.ToHtmlStringRGBA(color))
              .Append(str)
              .EndColor();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder StartFontSize(this StringBuilder sb, int fontSize)
        {
            return sb.Append(StartSizeTag)
                     .Append(fontSize.ToString())
                     .Append(EndTag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder EndFontSize(this StringBuilder sb, bool withEndLine = false)
        {
            return withEndLine ? sb.AppendLine(EndSizeTag) : sb.Append(EndSizeTag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder StartColor(this StringBuilder sb, string colorHex)
        {
            sb.Append(StartColorTag);
            if (colorHex.StartsWith(HashSymbol) == false)
            {
                sb.Append(HashSymbol);
            }
            sb.Append(colorHex)
              .Append(EndTag);

            return sb;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder EndColor(this StringBuilder sb, bool withEndLine = false)
        {
            return withEndLine ? sb.AppendLine(EndColorTag) : sb.Append(EndColorTag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RemoveTags(this string str)
        {
            var index1 = str.IndexOf(StartTag, StringComparison.Ordinal);
            var index2 = str.IndexOf(EndTag, StringComparison.Ordinal);

            while (index1 >= 0 && index2 >= 0)
            {
                str = str.Replace(str.Substring(index1, index2 - index1 + 1), string.Empty);

                index1 = str.IndexOf(StartTag, StringComparison.Ordinal);
                index2 = str.IndexOf(EndTag, StringComparison.Ordinal);
            }

            return str;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ColoredText(this string str, Color color)
        {
            var colorValue = ColorUtility.ToHtmlStringRGBA(color);
            return $"<color=#{colorValue}>{str}</color>";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CenterOnContent(this ScrollRect scrollView,
                                           RectTransform targetRect,
                                           float offset = 0f,
                                           bool forceRebuildLayout = false)
        {
            var scrollRectHeight = 0f;
            var totalHeight = scrollView.content.rect.height;

            if (scrollView.transform is RectTransform scrollRectTransform)
            {
                scrollRectHeight = scrollRectTransform.rect.height;
            }

            offset = (scrollRectHeight - targetRect.rect.height - offset) * 0.5f;

            if (forceRebuildLayout || totalHeight == 0)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
                totalHeight = scrollView.content.rect.height;
            }

            totalHeight -= scrollRectHeight;

            scrollView.verticalNormalizedPosition =
                Mathf.Clamp01(1f - (Mathf.Abs(targetRect.localPosition.y) - offset) / totalHeight);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RenderToTexture(this Camera camera, ref RenderTexture renderTexture)
        {
            if (renderTexture == null) return;
            
            var cachedActive = RenderTexture.active;

            camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;
            camera.Render();

            camera.targetTexture = null;
            RenderTexture.active = cachedActive;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetSiblingIndexAmongActive(this Transform transform, int index)
        {
            var parent = transform.parent;

            for (var i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.gameObject.activeSelf == false)
                {
                    index++;
                }
            }

            transform.SetSiblingIndex(index);
        }
    }
}