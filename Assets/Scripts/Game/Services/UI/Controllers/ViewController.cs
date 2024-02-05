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
    /// Абстрактный класс окна
    /// </summary>
    /// <typeparam name="TControllerParams"></typeparam>
    public abstract class ViewController<TControllerParams> : ViewController, IViewControllerWithParams
        where TControllerParams : IControllerParams
    {
        protected TControllerParams ViewParams { get; private set; }

        protected ViewController(IUnityComponent view, IUIService uiService, IControllerParams controllerParams) : base(view, uiService)
        {
            ViewParams = (TControllerParams)controllerParams;
        }

        void IViewControllerWithParams.UpdateParams(IControllerParams controllerParams)
        {
            if (controllerParams is TControllerParams controllerParamsCasted)
            {
                ViewParams = controllerParamsCasted;
            }
            else
            {
                throw new Exception($"Invalid params type: {controllerParams.GetType().Name}, required: {typeof(TControllerParams).Name}");
            }
        }
    }

    public abstract class ViewController : IViewController
    {
        protected IUIService UIService { get; }
        protected IUnityComponent View { get; private set; }
        protected List<IWidgetController> Widgets { get; } = new();
        protected IViewController Self => this;

        protected ViewController(IUnityComponent view, IUIService uiService)
        {
            UIService = uiService;
            View = view;
        }

        protected virtual void OnShown() { }
        protected virtual void OnInitialized() { }
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
            if (widget == null) return;

            if (!Widgets.Contains(widget)) return;

            widget.Hide();
            widget.Dispose();
            Widgets.Remove(widget);
        }

        protected void CloseSelf()
        {
            UIService.Close(this);
        }

        void IWidgetController.Initialize()
        {
            OnInitialized();
        }

        void IWidgetController.Show()
        {
            OnShown();
        }

        void IWidgetController.Hide()
        {
            OnHide();
        }

        async Task<TWidgetController> IWidgetController.CloneWidget<TWidgetController>(Transform parent,
                                                                                       IControllerParams controllerParams,
                                                                                       string group) where TWidgetController : class
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
                                                                                             IControllerParams controllerParams,
                                                                                             string group) where TWidgetController : class
        {
            var widgetController = await UIService.InstantiateWidget<TWidgetController>(parent, controllerParams);
            widgetController.Initialize();
            widgetController.Show();
            Widgets.Add(widgetController);
            return widgetController;
        }


        async Task<TWidgetController> IWidgetController.InstantiateWidget<TWidgetController>(string name,
                                                                                             Transform parent,
                                                                                             IControllerParams controllerParams,
                                                                                             string group) where TWidgetController : class
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
                ClearWidgets();
                OnDispose();
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