using System;
using System.Threading.Tasks;
using Plugins.AssetsReference;
using Runner.Game.Services.UI.Params;
using UnityEngine;

namespace Runner.Game.Services.UI.Interfaces
{
    public interface IWidgetController : IDisposable
    {
        void Initialize();
        void Show();
        void Hide();
        Task<TWidgetController> CloneWidget<TWidgetController>(Transform parent, IControllerParams controllerParams = null, string group = null) where TWidgetController : class, IWidgetController;
        TWidgetController WrapWidget<TWidgetController>(IUnityComponent widget, IControllerParams controllerParams = null) where TWidgetController : class, IWidgetController;
        TWidgetController WrapWidget<TWidgetController>(IUnityComponent widget, string name, IControllerParams controllerParams = null) where TWidgetController : class, IWidgetController;
        Task<TWidgetController> InstantiateWidget<TWidgetController>(Transform parent, IControllerParams controllerParams = null, string group = null) where TWidgetController : class, IWidgetController;
        Task<TWidgetController> InstantiateWidget<TWidgetController>(string name, Transform parent, IControllerParams controllerParams = null, string group = null) where TWidgetController : class, IWidgetController;
    }
    
    public interface IWidgetControllerWithParams : IWidgetController
    {
        void UpdateParams(IControllerParams controllerParams);
    }
}