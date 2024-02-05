using System;
using System.Reflection;
using System.Threading.Tasks;
using Runner.Core.Services.Cache;
using Plugins.AssetsReference;
using UnityEngine;

namespace Runner.Game.Services.UI
{
    /// <summary>
    /// Фабрика для создания виджетов и окон через кэш систему
    /// </summary>
    public sealed class UIFactory : IUIFactory
    {
        private readonly ICacheService _cacheService;

        public UIFactory(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        async Task<IUnityComponent> IUIFactory.Clone<TViewController>(IUnityComponent parentView, Transform parent)
        {
            var viewName = GetViewName(typeof(TViewController));
            var sourceComponent = parentView.Get<UnityComponent>(viewName) as IUnityComponent;
            
            Debug.Assert(sourceComponent != null, $"[{nameof(UIFactory)}] Can't find {viewName} view. Please add {viewName} to parent {parentView.GameObject.name} unity components list");
            
            var source = sourceComponent.GameObject;
            source.SetActive(false);
            
            var instance = await _cacheService.Get<GameObject>(source, parent);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;
            instance.SetActive(true);

            return instance.GetComponent<IUnityComponent>();
        }
        
        IUnityComponent IUIFactory.Wrap<TViewController>(IUnityComponent view)
        {
            var viewName = GetViewName(typeof(TViewController));
            return view.Get<UnityComponent>(viewName);
        }

        async Task<IUnityComponent> IUIFactory.Instantiate<TViewController>(Transform parent, string name)
        {
            var resourceKey = string.IsNullOrEmpty(name) == false ? name : GetViewName(typeof(TViewController));
            var view = await _cacheService.Get<string>(resourceKey, parent);
            return view.GetComponent<IUnityComponent>();
        }
        
        private string GetViewName(MemberInfo viewType)
        {
            if (viewType.GetCustomAttribute(typeof(ViewAttribute)) is not ViewAttribute viewAttribute)
            {
                throw new Exception($"{viewType.Name} has no {nameof(ViewAttribute)}");
            }
           
            return viewAttribute.Name;
        }
    }
}