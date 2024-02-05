using System.Threading.Tasks;
using Plugins.AssetsReference;
using UnityEngine;

namespace Runner.Game.Services.UI
{
    public interface IUIFactory
    {
        Task<IUnityComponent> Clone<TViewController>(IUnityComponent parentView, Transform parent);
        IUnityComponent Wrap<TViewController>(IUnityComponent view);
        Task<IUnityComponent> Instantiate<TViewController>(Transform parent, string name = null);
    }
}