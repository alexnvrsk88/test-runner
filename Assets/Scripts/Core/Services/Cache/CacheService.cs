using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Extend;
using Runner.Core.Services.Cache.CacheProviders;
using UnityEngine;

namespace Runner.Core.Services.Cache
{
    /// <summary>
    /// Сервис кэширования ресурсов.
    /// Сервис может содержать разные типы провайдеров для работы с ресурсами.
    /// </summary>
    [Injection(true, typeof(ICacheService))]
    public sealed class CacheService : ServiceAbstract, ICacheService
    {
        private readonly Dictionary<Type, ICacheProvider> _providers = new();
        private readonly IExportLocatorScope _container;

        private string _currentGroup;

        public CacheService(IExportLocatorScope container)
        {
            _container = container;
            
            CreateProvider<GameObjectCacheProvider>();
            CreateProvider<ResourcesCacheProvider>();
            CreateProvider<AssetReferenceCacheProvider>();
        }

        public override Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        private void CreateProvider<TProvider>() where TProvider : ICacheProvider
        {
            var provider = _container.Locate<TProvider>();
            _providers.Add(provider.CachedType, provider);
        }

        async Task<GameObject> ICacheService.Get<T>(object key, Transform parent)
        {
            if (_providers.TryGetValue(typeof(T), out var provider))
            {
                return await provider.Get(key, parent);
            }

            throw new Exception($"Not found cache provider for {typeof(T).Name}");
        }

        void ICacheService.Return(GameObject gameObject)
        {
            foreach (var (_, cacheProvider) in _providers)
            {
                if (cacheProvider.Return(gameObject))
                {
                    break;
                }
            }
        }

        void ICacheService.ReturnDeferred(GameObject gameObject, float delay, Action onReturn)
        {
            foreach (var (_, cacheProvider) in _providers)
            {
                if (cacheProvider.ReturnDeferred(gameObject, delay, onReturn))
                {
                    break;
                }
            }
        }

        void ICacheService.ClearAll(bool forced)
        {
            foreach (var (_, cacheProvider) in _providers)
            {
                cacheProvider.Clear(forced);
            }
        }
    }
}