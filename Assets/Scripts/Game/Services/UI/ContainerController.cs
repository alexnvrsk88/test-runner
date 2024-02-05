using System;
using System.Collections.Generic;
using Runner.Core.Services.Logging;
using Plugins.AssetsReference;
using UnityEngine;
using UnityEngine.UI;

namespace Runner.Game.Services.UI
{
    /// <summary>
    /// Базовые контроллер для UI, отвечает за порядок UI слоев и применение SafeArea 
    /// </summary>
    public sealed class RootUiContainerController
    {
        private const string ROOT_CANVAS_ID = "RootCanvas";
        private const string CAMERA_ID = "Camera";
        private const string SAFE_AREA_CONTROLLER_ID = "SafeAreaController";
        private const string SAFE_AREA_CONTAINER_NAME = "SafeArea";

        public Camera Camera { get; }
        public Canvas Canvas { get; }
        
        private readonly SafeAreaController _safeAreaController;
        private readonly ILoggingService _logger;
        private readonly Dictionary<ViewLayer, Transform> _layers = new();

        public RootUiContainerController(IUnityComponent component, ILoggingService logger)
        {
            _logger = logger;
            
            Camera = component.Get<Camera>(CAMERA_ID);
            Canvas = component.Get<Canvas>(ROOT_CANVAS_ID);
            _safeAreaController = component.Get<SafeAreaController>(SAFE_AREA_CONTROLLER_ID);
        }

        public void Initialize()
        {
            foreach (Transform child in Canvas.transform)
            {
                if (Enum.TryParse<ViewLayer>(child.name, out var viewLayer))
                {
                    var canvas = child.gameObject.AddComponent<Canvas>();
                    child.gameObject.AddComponent<GraphicRaycaster>();
                    
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = (int)viewLayer;
                    
                    _layers[viewLayer] = child;
                }
                else
                {
                    _logger.Info($"[{nameof(RootUiContainerController)}] You forget to add ViewLayer with name {child.name} to template. Such layer will not work!");
                }
            }
        }

        public Transform GetLayer(ViewLayer layerType)
        {
            if (_layers.TryGetValue(layerType, out var transform))
            {
                return transform;
            }

            _logger.Error($"[{nameof(RootUiContainerController)}] Can't find layer {layerType}! Will use {ViewLayer.Default} layer.");
            return _layers[ViewLayer.Default];
        }

        public void TryApplySafeArea(RectTransform rectTransform)
        {
            if (TryGetSafeArea(rectTransform, searchDepth: 1, out var safeAreaTransform))
            {
                _safeAreaController.ApplySafeArea(safeAreaTransform);
            }
        }

        private bool TryGetSafeArea(RectTransform rectTransform, int searchDepth, out RectTransform safeAreaTransform)
        {
            safeAreaTransform = null;

            if (rectTransform.name == SAFE_AREA_CONTAINER_NAME)
            {
                safeAreaTransform = rectTransform;
                return true;
            }

            if (searchDepth == 0)
            {
                return false;
            }

            for (var i = 0; i < rectTransform.childCount; i++)
            {
                var child = rectTransform.GetChild(i);

                if (TryGetSafeArea(child as RectTransform, searchDepth - 1, out safeAreaTransform))
                {
                    return true;
                }
            }

            return false;
        }
    }
}