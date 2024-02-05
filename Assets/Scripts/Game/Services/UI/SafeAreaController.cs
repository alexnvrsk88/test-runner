using UnityEngine;

namespace Runner.Game.Services.UI
{
    [RequireComponent(typeof(Canvas))]
    public class SafeAreaController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        public void ApplySafeArea(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return;
            }

            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            var canvasPixelRect = _canvas.pixelRect;
        
            anchorMin.x /= canvasPixelRect.width;
            anchorMin.y /= canvasPixelRect.height;
            anchorMax.x /= canvasPixelRect.width;
            anchorMax.y /= canvasPixelRect.height;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }

        private void OnValidate()
        {
            if (!_canvas)
            {
                _canvas = GetComponent<Canvas>();
            }
        }
    }
}