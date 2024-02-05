using System;
using System.Threading.Tasks;
using Runner.Core.Services.Logging;
using Runner.Core.Services.Ticks;
using UnityEngine;

namespace Runner.Core.Services.Cache.CacheProviders
{
    /// <summary>
    /// Кэш провайдер для геймобъектов
    /// </summary>
    public sealed class GameObjectCacheProvider : CacheProvider
    {
        public override Type CachedType => typeof(GameObject);

        public GameObjectCacheProvider(ITickService tickService, ILoggingService loggingService) : base(tickService, loggingService) { }

        protected override Task<GameObject> CreateInstanceInternal(object owner, object cachedKey, bool wake)
        {
            var gameObject = (GameObject)cachedKey;
            var parent = wake ? ActiveCacheContainer : InactiveCacheContainer;
            var instance = UnityEngine.Object.Instantiate(gameObject, parent);
            return Task.FromResult(instance);
        }

        protected override void Release(object owner, GameObject gameObject)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}