using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugins.AssetsReference;
using Runner.Game.Services.UI.Interfaces;
using Runner.Game.Services.UI.Params;
using UnityEngine;

namespace Runner.Game.Services.UI.Controllers
{
    /// <summary>
    /// Абстрактный класс виджета с параметрами
    /// </summary>
    /// <typeparam name="TControllerParams"></typeparam>
    public abstract class WidgetController<TControllerParams> : WidgetController, IWidgetControllerWithParams
        where TControllerParams : IControllerParams
    {
        protected TControllerParams WidgetParams { get; private set; }

        protected WidgetController(IUnityComponent view, IControllerParams controllerParams, IUIService uiService) : base(view, uiService)
        {
            WidgetParams = (TControllerParams)controllerParams;
        }

        public void UpdateParams(IControllerParams controllerParams)
        {
            WidgetParams = (TControllerParams)controllerParams;
            OnUpdated();
        }
    }

    /// <summary>
    /// Абстрактный класс виджета
    /// </summary>
    public abstract class WidgetController : IWidgetController
    {
        protected readonly IUIService UIService;

        private readonly int _index;

        protected List<IWidgetController> Widgets { get; } = new();
        protected IUnityComponent View { get; private set; }
        protected IWidgetController Self => this;

        protected WidgetController(IUnityComponent view, IUIService uiService)
        {
            UIService = uiService;
            View = view;
        }

        public void Show()
        {
            View.GameObject.SetActive(true);
            OnShown();
        }

        public void Hide()
        {
            OnHide();
            View.GameObject.SetActive(false);
        }

        protected virtual void OnInitialized() { }
        protected virtual void OnShown() { }
        protected virtual void OnHide() { }
        protected virtual void OnUpdated() { }
        protected virtual void OnDispose() { }

        protected void ClearWidgets()
        {
            foreach (var widget in Widgets)
            {
                widget.Dispose();
            }

            Widgets.Clear();
        }

        protected void DisposeWidget(IWidgetController widget)
        {
            if (widget == null || !Widgets.Contains(widget))
            {
                return;
            }

            try
            {
                widget.Dispose();
                Widgets.Remove(widget);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        void IWidgetController.Initialize()
        {
            OnInitialized();
        }

        async Task<TWidgetController> IWidgetController.CloneWidget<TWidgetController>(Transform parent,
            IControllerParams controllerParams, string group) where TWidgetController : class
        {
            var widgetController = await UIService.CloneWidget<TWidgetController>(View, parent, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }

        TWidgetController IWidgetController.WrapWidget<TWidgetController>(IUnityComponent widget, IControllerParams controllerParams) where TWidgetController : class
        {
            var widgetController = UIService.WrapWidget<TWidgetController>(widget, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }

        TWidgetController IWidgetController.WrapWidget<TWidgetController>(IUnityComponent widget, string name, IControllerParams controllerParams) where TWidgetController : class
        {
            var widgetController = UIService.WrapWidget<TWidgetController>(widget, name, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }

        async Task<TWidgetController> IWidgetController.InstantiateWidget<TWidgetController>(Transform parent,
            IControllerParams controllerParams, string group) where TWidgetController : class
        {
            var widgetController = await UIService.InstantiateWidget<TWidgetController>(parent, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }
        
        async Task<TWidgetController> IWidgetController.InstantiateWidget<TWidgetController>(string name, Transform parent,
            IControllerParams controllerParams, string group) where TWidgetController : class
        {
            var widgetController = await UIService.InstantiateWidget<TWidgetController>(name, parent, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }

        void IDisposable.Dispose()
        {
            try
            {
                OnDispose();
                ClearWidgets();
                UIService.ReleaseView(View);
                View = null;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}