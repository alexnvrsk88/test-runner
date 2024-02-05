using System;
using System.Threading.Tasks;
using Runner.Core.Services.Logging;
using Runner.Core.Services.Resource;
using Runner.Core.Services.Ticks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Runner.Core.Services.Cache.CacheProviders
{
    /// <summary>
    /// Кэш провайдер ресурсов для ссылок AssetReference
    /// </summary>
    public sealed class AssetReferenceCacheProvider : CacheProvider
    {
        private readonly IResourcesService _resourcesService;

        public AssetReferenceCacheProvider(ITickService tickService, ILoggingService loggingService, IResourcesService resourcesService) : base(tickService, loggingService)
        {
            _resourcesService = resourcesService;
        }

        public override Type CachedType => typeof(AssetReference);

        protected override async Task<GameObject> CreateInstanceInternal(object owner, object cachedKey, bool wake)
        {
            var resourceKey = (AssetReference)cachedKey;
            var resourceData = new ResourceData(false, resourceKey.SubObjectName);
            var parent = wake ? ActiveCacheContainer : InactiveCacheContainer;
            var instantiationParameters = new InstantiationParameters(parent, false);
            var instance = await _resourcesService.InstantiateAsync<GameObject>(owner, resourceKey, resourceData, instantiationParameters);

            return instance;
        }

        protected override void Release(object owner, GameObject gameObject)
        {
            _resourcesService.ReleaseInstance(owner, gameObject);
        }
    }
}