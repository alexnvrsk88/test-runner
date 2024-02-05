// #define HIDE_CACHED_OBJECTS_IN_HIERARCHY

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Runner.Core.Utils;
using Runner.Core.Services.Logging;
using Runner.Core.Services.Ticks;
using UnityEngine;

namespace Runner.Core.Services.Cache
{
    /// <summary>
    /// Абстрактный класс кэш провайдера для кэширования ресурсов.
    /// Насоследники реализуют свой тип индекса (адреса) ресурса
    /// </summary>
    public abstract class CacheProvider : ITickable, ICacheProvider
    {
        private const string PARENT_CONTAINER_NAME = "[CACHE]";

        protected readonly Transform InactiveCacheContainer;
        protected readonly Transform ActiveCacheContainer;

        private readonly Dictionary<object, Stack<GameObject>> _cacheMap = new();
        private readonly Dictionary<GameObject, object> _borrowed = new();
        private readonly List<DeferredReturn> _deferred = new();
        private readonly Stack<DeferredReturn> _deferredEntriesPool = new();
        private readonly List<GameObject> _all = new();

        private readonly ITickService _tickService;
        private readonly ILoggingService _loggingService;

        private ICacheProvider Self => this;

        public abstract Type CachedType { get; }

        protected CacheProvider(ITickService tickService, ILoggingService loggingService)
        {
            tickService.Subscribe(this);
            _loggingService = loggingService;

            var rootContainer = GameObject.Find(PARENT_CONTAINER_NAME);
            if (rootContainer == null)
            {
                rootContainer = new GameObject(PARENT_CONTAINER_NAME);
                rootContainer.transform.position = Vector3.down * 9999f;
                UnityEngine.Object.DontDestroyOnLoad(rootContainer);
            }

            InactiveCacheContainer = new GameObject($"{GetType().Name} [Inactive]").transform;
            InactiveCacheContainer.gameObject.SetActive(false);
            InactiveCacheContainer.transform.position = Vector3.down * 9999f;
            InactiveCacheContainer.SetParent(rootContainer.transform);

            ActiveCacheContainer = new GameObject($"{GetType().Name} [Active]").transform;
            ActiveCacheContainer.gameObject.SetActive(true);
            ActiveCacheContainer.transform.position = Vector3.down * 9999f;
            ActiveCacheContainer.SetParent(rootContainer.transform);
        }

        void ITickable.OnTick(float deltaTime)
        {
            for (var i = 0; i < _deferred.Count; i++)
            {
                var deferred = _deferred[i];

                deferred.Delay -= deltaTime;

                if (deferred.Delay > 0.0f) continue;

                _deferred.RemoveBySwap(i);

                if (!_borrowed.ContainsKey(deferred.GameObject))
                {
                    continue;
                }

                deferred.Callback.SafeInvoke();
                Self.Return(deferred.GameObject);
                deferred.Reset();

                _deferredEntriesPool.Push(deferred);
                i--;
            }
        }

        async Task<GameObject> ICacheProvider.Get(object cached,
            Transform parent,
            bool createIfEmpty,
            bool wake)
        {
            if (_cacheMap.TryGetValue(cached, out var stack) == false)
            {
                stack = new Stack<GameObject>();
                _cacheMap[cached] = stack;
            }

            if (stack.Count == 0)
            {
                if (createIfEmpty)
                {
                    await CreateInstance(cached, wake);
                }
                else
                {
                    _loggingService.Warning($"Object <{nameof(CachedType)}> not available in cache, returning NULL");
                    return null;
                }
            }

            var instance = stack.Pop();
            _borrowed[instance] = cached;

            var instanceTransform = instance.transform;
            instanceTransform.SetParent(parent, false);
            instanceTransform.localPosition = Vector3.zero;
            instanceTransform.localRotation = Quaternion.identity;
            instanceTransform.localScale = Vector3.one;
            instance.SetActive(true);

#if UNITY_EDITOR && HIDE_CACHED_OBJECTS_IN_HIERARCHY
            instance.hideFlags &= ~HideFlags.HideInHierarchy;
#endif
            return instance;
        }

        bool ICacheProvider.Return(GameObject instance)
        {
            if (_borrowed.ContainsKey(instance) == false)
            {
                return false;
            }

            var instanceTransform = instance.transform;
            if (instanceTransform.parent != InactiveCacheContainer)
            {
                instanceTransform.SetParent(InactiveCacheContainer, false);
            }
            
            _cacheMap[_borrowed[instance]].Push(instance);
            _borrowed.Remove(instance);

#if UNITY_EDITOR && HIDE_CACHED_OBJECTS_IN_HIERARCHY
            if (instance != null)
            {
                instance.hideFlags |= HideFlags.HideInHierarchy;
            }
#endif

            return true;
        }

        bool ICacheProvider.ReturnDeferred(GameObject instance, float delay, Action callback)
        {
            if (_borrowed.ContainsKey(instance) == false) return false;

            var toReturn = _deferredEntriesPool.Count > 0
                ? _deferredEntriesPool.Pop().Setup(instance, delay)
                : new DeferredReturn(instance, delay);

            toReturn.Callback = callback;

            _deferred.Add(toReturn);
            return true;
        }

        void ICacheProvider.Clear(bool forced)
        {
            if (forced)
            {
                _deferred.Clear();
                _borrowed.Clear();
                _cacheMap.Clear();

                foreach (var instance in _all)
                {
                    Release(this, instance);
                }

                _all.Clear();
                return;
            }

            foreach (var stack in _cacheMap.Values)
            {
                while (stack.TryPop(out var gameObject))
                {
                    ReleaseInstance(gameObject);
                }
            }

            foreach (var deferredReturn in _deferred)
            {
                ReleaseInstance(deferredReturn.GameObject);

                deferredReturn.Reset();

                _deferredEntriesPool.Push(deferredReturn);
            }

            _deferred.Clear();

            for (var i = 0; i < _all.Count; i++)
            {
                var gameObject = _all[i];

                if (_borrowed.ContainsKey(gameObject) == false)
                {
                    ReleaseInstance(gameObject);
                }
            }
        }

        void ICacheProvider.ClearCached()
        {
            foreach (var stack in _cacheMap.Values)
            {
                while (stack.TryPop(out var gameObject))
                {
                    ReleaseInstance(gameObject);
                }
            }
        }

        int ICacheProvider.GetCachedCount(object cachedKey)
        {
            if (!_cacheMap.TryGetValue(cachedKey, out var stack))
            {
                return 0;
            }

            return stack.Count;
        }

        string ICacheProvider.LogStatus()
        {
            var sb = new StringBuilder();

            foreach (var gameObject in _all.OrderBy(o => o.name))
            {
                sb.AppendLine(gameObject.name);
            }

            return sb.ToString();
        }

        protected abstract Task<GameObject> CreateInstanceInternal(object owner, object cachedKey, bool wake);
        protected abstract void Release(object owner, GameObject gameObject);

        private async Task CreateInstance(object cachedKey, bool wake)
        {
            var instance = await CreateInstanceInternal(this, cachedKey, wake);

            var instanceTransform = instance.transform;
            if (instanceTransform.parent == ActiveCacheContainer)
            {
                instanceTransform.SetParent(InactiveCacheContainer);
            }

            _cacheMap[cachedKey].Push(instance);
            _all.Add(instance);

#if UNITY_EDITOR && HIDE_CACHED_OBJECTS_IN_HIERARCHY
            instance.hideFlags |= HideFlags.HideInHierarchy;
#endif
        }

        private void ReleaseInstance(GameObject go)
        {
            _all.RemoveBySwap(go);
            Release(this, go);
        }

        private sealed class DeferredReturn
        {
            public GameObject GameObject;
            public object Cached;
            public float Delay;
            public Action Callback;

            public DeferredReturn(GameObject gameObject, float delay)
            {
                Setup(gameObject, delay);
            }

            public DeferredReturn Setup(GameObject gameObject, float delay)
            {
                GameObject = gameObject;
                Delay = delay;
                return this;
            }

            public void Reset()
            {
                GameObject = default;
                Delay = 0.0f;
                Callback = null;
            }
        }
    }
}