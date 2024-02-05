using UnityEngine;

namespace Runner.Game.UI.Widgets
{
    // Makes current RectTransform fill entire Canvas, ignoring SafeArea
    public class IgnoreSafeArea : MonoBehaviour
    {
        [SerializeField] private bool _useSafeAreaLocalX;
        [SerializeField] private bool _useSafeAreaLocalY;

        private Canvas _canvas;
        private RectTransform _rectTransform;

        [ContextMenu("Update size")]
        public void UpdateSize()
        {
            UpdateReferences();

            if (_rectTransform == null || _canvas == null)
            {
                return;
            }

            var parent = transform.parent;
            var safeAreaTransform = GetSafeAreaTransform(parent);
            var worldCenter = safeAreaTransform.parent.TransformPoint(Vector3.zero);
            var localCenter = parent.InverseTransformPoint(worldCenter);
            var fullScreenSize = _canvas.pixelRect.size / _canvas.scaleFactor;

            if (_useSafeAreaLocalX)
            {
                localCenter.x = safeAreaTransform.localPosition.x;
            }

            if (_useSafeAreaLocalY)
            {
                localCenter.y = safeAreaTransform.localPosition.y;
            }

            _rectTransform.pivot = Vector2.one * 0.5f;
            _rectTransform.anchorMin = Vector2.one * 0.5f;
            _rectTransform.anchorMax = Vector2.one * 0.5f;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullScreenSize.x);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, fullScreenSize.y);
            _rectTransform.localPosition = localCenter;
        }

        private void UpdateReferences()
        {
            _rectTransform ??= GetComponent<RectTransform>();
            _canvas ??= _rectTransform.parent.GetComponentInParent<Canvas>();
        }

        private Transform GetSafeAreaTransform(Transform parent)
        {
            while (true)
            {
                if (parent == null)
                {
                    return null;
                }

                if (parent.name == "SafeArea")
                {
                    return parent;
                }
                parent = parent.parent;
            }
        }
    }
}