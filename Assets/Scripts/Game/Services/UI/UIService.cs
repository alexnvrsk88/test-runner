using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Extend;
using Runner.Core.Services.Cache;
using Runner.Core.Utils;
using Plugins.AssetsReference;
using Runner.Core.Services.Resource;
using Runner.Game.Services.UI.Interfaces;
using Runner.Game.Services.UI.Params;
using UnityEngine;

namespace Runner.Game.Services.UI
{
    public enum ViewLayer
    {
        Default = 0,
        Loader = 100,
        Overlay = 200
    }

    /// <summary>
    /// UI сервис служит для работы с окнами и виджетами
    /// </summary>
    [Injection(true, typeof(IUIService))]
    public sealed class UIService : GameServiceAbstract, IUIService
    {
        private const string VIEW_CONSTRUCTOR_PARAM = "view";
        private const string PARAMS_CONSTRUCTOR_PARAM = "controllerParams";
        private const string TEMPLATE_ASSET_NAME = "UIRoot";

        private readonly IResourcesService _resourcesService;
        private readonly IExportLocatorScope _container;
        private readonly IUIFactory _uiFactory;
        private readonly ICacheService _cacheService;

        private RootUiContainerController _containerController;
        private IUnityComponent _rootCanvas;
        private RectTransform _rootCanvasTransform;
        private GameObject _canvasRefreshObject;

        private readonly Dictionary<Type, IViewController> _activeControllers = new();

        public event Action<IViewController> OnViewShown;
        public event Action<Type> OnViewClosed;

        Canvas IUIService.RootCanvas => _containerController.Canvas;
        RectTransform IUIService.RootCanvasTransform => _rootCanvasTransform;

        public UIService(IExportLocatorScope container,
                         IResourcesService resourcesService,
                         IUIFactory uiFactory,
                         ICacheService cacheService)
        {
            _container = container;
            _resourcesService = resourcesService;
            _uiFactory = uiFactory;
            _cacheService = cacheService;
        }

        public override async Task<bool> Initialize()
        {
            _rootCanvas = await _resourcesService.InstantiateAsync<IUnityComponent>(this, TEMPLATE_ASSET_NAME, new ResourceData(true, "[UI]"));
            _containerController = _container.Locate<RootUiContainerController>(new Dictionary<string, object> { { "component", _rootCanvas } });
            _containerController.Initialize();
            
            _rootCanvasTransform = _containerController.Canvas.transform as RectTransform;
            
            return true;
        }

        /// <summary>
        /// Получение уже сущесвтующего котроллера окна 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUIService.GetView<T>()
        {
            if (_activeControllers.TryGetValue(typeof(T), out var controller))
            {
                return (T)controller;
            }

            return null;
        }

        /// <summary>
        /// Проверка активно окно или нет
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IUIService.IsViewActive<T>()
        {
            return _activeControllers.TryGetValue(typeof(T), out _);
        }

        /// <summary>
        /// Отображение окна, в случае если оно уже активно, то показываем существующее
        /// </summary>
        /// <param name="controllerParams"></param>
        /// <param name="parent"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<T> Show<T>(IControllerParams controllerParams, Transform parent = null)
            where T : class, IViewController
        {
            if (_activeControllers.TryGetValue(typeof(T), out var controller))
            {
                if (controllerParams != null && controller is IViewControllerWithParams viewControllerParameterized)
                {
                    viewControllerParameterized.UpdateParams(controllerParams);
                }

                controller.Show();

                OnViewShown.SafeInvoke(controller);

                return (T)controller;
            }

            var controllerName = typeof(T).Name;
            try
            {
                var view = await _uiFactory.Instantiate<T>(parent == null ? _containerController.GetLayer(GetViewLayer(typeof(T))) : parent);
                _containerController.TryApplySafeArea(view.RectTransform);
                view.ActiveController = controllerName;

                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, view }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
                
                controller.Initialize();
                controller.Show();
                _activeControllers.Add(typeof(T), controller);
            }
            catch (Exception e)
            {
                Debug.LogException(new Exception($"[{nameof(UIService)}] Failed show {controllerName}"));
                Debug.LogException(e);
            }

            OnViewShown.SafeInvoke(controller);

            return (T)controller;
        }

        /// <summary>
        /// Скопировать виджет из существущего референса (геймоъекта)
        /// </summary>
        /// <param name="widgetOrigin"></param>
        /// <param name="parent"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        async Task<T> IUIService.CloneWidget<T>(IUnityComponent widgetOrigin,
            Transform parent,
            IControllerParams controllerParams)
        {
            T controller = default;

            try
            {
                var widget = await _uiFactory.Clone<T>(widgetOrigin, parent);
                widget.ActiveController = typeof(T).Name;
                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return controller;
        }

        /// <summary>
        /// Оборачивание уже инстанцированного виджета (геймобъекта)
        /// </summary>
        /// <param name="parentView"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUIService.WrapWidget<T>(IUnityComponent parentView, IControllerParams controllerParams)
        {
            T controller = default;

            try
            {
                var widget = _uiFactory.Wrap<T>(parentView);
                widget.ActiveController = typeof(T).Name;
                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return controller;
        }

        /// <summary>
        /// Оборачивание уже инстанцированного виджета (геймобъекта)
        /// </summary>
        /// <param name="parentView"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUIService.WrapWidget<T>(IUnityComponent parentView, string name, IControllerParams controllerParams)
        {
            T controller = default;

            try
            {
                var widget = parentView.Get<UnityComponent>(name) as IUnityComponent;
                widget.ActiveController = typeof(T).Name;
                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return controller;
        }

        /// <summary>
        /// Создание нового виджета
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        async Task<T> IUIService.InstantiateWidget<T>(Transform parent, IControllerParams controllerParams)
        {
            T controller = default;

            try
            {
                var widget = await _uiFactory.Instantiate<T>(parent);
                widget.ActiveController = typeof(T).Name;
                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return controller;
        }

        /// <summary>
        /// Создание нового виджета
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        async Task<T> IUIService.InstantiateWidget<T>(string name, Transform parent, IControllerParams controllerParams)
        {
            T controller = default;

            try
            {
                var widget = await _uiFactory.Instantiate<T>(parent, name);
                widget.ActiveController = typeof(T).Name;
                controller = _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return controller;
        }

        /// <summary>
        /// Закрытие окна по типу
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void IUIService.Close<T>()
        {
            var viewControllerType = typeof(T);
            if (!_activeControllers.TryGetValue(viewControllerType, out var controller))
            {
                return;
            }

            controller.Hide();
            controller.Dispose();
            
            _activeControllers.Remove(viewControllerType);
            
            OnViewClosed.SafeInvoke(viewControllerType);
        }

        /// <summary>
        /// Закрытия экземпляра окна
        /// </summary>
        /// <param name="viewController"></param>
        void IUIService.Close(IViewController viewController)
        {
            var viewControllerType = viewController.GetType();
            if (!_activeControllers.TryGetValue(viewControllerType, out var controller))
            {
                return;
            }

            controller.Hide();
            controller.Dispose();
            
            _activeControllers.Remove(viewControllerType);
            
            OnViewClosed.SafeInvoke(viewControllerType);
        }

        /// <summary>
        /// Удаление окна со сцены
        /// </summary>
        /// <param name="unityComponent"></param>
        void IUIService.ReleaseView(IUnityComponent unityComponent)
        {
            if (unityComponent is UnityComponent uc && uc != null && unityComponent.IsAlive)
            {
                unityComponent.ActiveController = string.Empty;
                _cacheService.Return(unityComponent.GameObject);
            }
        }

        /// <summary>
        /// Показ виджета
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T IUIService.ShowWidget<T>(IUnityComponent widget, IControllerParams controllerParams)
        {
            var widgetController = (IWidgetController)GetController<T>(widget, controllerParams);
            widgetController.Initialize();
            widgetController.Show();

            return (T)widgetController;
        }

        /// <summary>
        /// Создание контроллера
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="controllerParams"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private T GetController<T>(IUnityComponent widget, IControllerParams controllerParams)
        {
            return _container.Locate<T>(new Dictionary<string, object> { { VIEW_CONSTRUCTOR_PARAM, widget }, { PARAMS_CONSTRUCTOR_PARAM, controllerParams } });
        }

        /// <summary>
        /// Получение UI слоя для окна, заданного через атрибут класса View  
        /// </summary>
        /// <param name="viewType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private ViewLayer GetViewLayer(MemberInfo viewType)
        {
            if (viewType.GetCustomAttribute(typeof(ViewAttribute)) is not ViewAttribute viewAttribute)
            {
                throw new Exception($"{viewType.Name} has no {nameof(ViewAttribute)}");
            }

            return viewAttribute.Layer;
        }
    }
}