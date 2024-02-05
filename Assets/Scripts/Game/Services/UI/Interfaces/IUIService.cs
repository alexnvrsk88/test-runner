using System;
using System.Threading.Tasks;
using Plugins.AssetsReference;
using Runner.Game.Services.UI.Params;
using UnityEngine;

namespace Runner.Game.Services.UI.Interfaces
{
    public interface IUIService
    {
        event Action<IViewController> OnViewShown;
        event Action<Type> OnViewClosed;

        Canvas RootCanvas { get; }
        RectTransform RootCanvasTransform { get; }

        Task<T> Show<T>(IControllerParams @params = null, Transform container = null) where T : class, IViewController;
        Task<T> CloneWidget<T>(IUnityComponent unityComponent, Transform parent, IControllerParams controllerParams = null);
        T WrapWidget<T>(IUnityComponent viewToWrap, IControllerParams controllerParams = null);
        T WrapWidget<T>(IUnityComponent parentView, string name, IControllerParams controllerParams = null);
        Task<T> InstantiateWidget<T>(Transform parent, IControllerParams controllerParams = null);
        Task<T> InstantiateWidget<T>(string name, Transform parent, IControllerParams controllerParams = null);
        void Close<T>() where T : IViewController;
        void Close(IViewController viewController);
        void ReleaseView(IUnityComponent unityComponent);
        T GetView<T>() where T : class, IViewController;
        bool IsViewActive<T>() where T : IViewController;
        T ShowWidget<T>(IUnityComponent widget, IControllerParams controllerParams);
    }
}