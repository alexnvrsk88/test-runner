using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grace.Extend;
using Runner.Core.Services.Logging;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Runner.Core.Services.Resource
{
    /// <summary>
    /// Сервис для работы с ресурсами.
    /// Загрузка, отгрузка через Addressables
    /// </summary>
    [Injection(true, typeof(IResourcesService))]
    public sealed class ResourcesService : ServiceAbstract, IResourcesService
    {
        private const string ADDRESSABLES_LOCAL_LABEL = "LocalAsset";

        private readonly Dictionary<string, IResourceLocation> _resourcesLocations = new();
        private readonly Dictionary<string, AsyncOperationsContainer> _containers = new();
        private readonly Dictionary<GameObject, string> _instanceKeys = new();
        private readonly ILoggingService _logger;

        public ResourcesService(ILoggingService logger)
        {
            _logger = logger;
        }

        public override async Task<bool> Initialize()
        {
            await Addressables.InitializeAsync().Task;

            var resourcesLocationsList = await Addressables.LoadResourceLocationsAsync(ADDRESSABLES_LOCAL_LABEL).Task;

            foreach (var resourcesLocation in resourcesLocationsList)
            {
                _resourcesLocations.Add(resourcesLocation.PrimaryKey, resourcesLocation);
            }

            _logger.Info($"{nameof(ResourcesService)} initialized with {_resourcesLocations.Count} resources");
            return true;
        }

        async Task<T> IResourcesService.LoadAsync<T>(object owner, string assetName)
        {
            var resourceLocation = _resourcesLocations.GetValueOrDefault(assetName);
            var aoh = Addressables.LoadAssetAsync<T>(resourceLocation);
            
            GetAssetContainer(assetName).Add(owner, aoh);
            
            await aoh.Task;

            return aoh.Result;
        }

        async Task<T> IResourcesService.LoadAsync<T>(object owner, AssetReference assetReference)
        {
            var aoh = Addressables.LoadAssetAsync<T>(assetReference);

            GetAssetContainer(assetReference.AssetGUID).Add(owner, aoh);

            await aoh.Task;

            return aoh.Result;
        }

        async Task<SceneInstance> IResourcesService.LoadSceneAsync(object owner, string sceneName, LoadSceneMode mode)
        {
            if (owner == null)
            {
                _logger.Error($"[{nameof(ResourcesService)}] [{nameof(IResourcesService.LoadSceneAsync)}] Argument {nameof(owner)} is null");
                return default;
            }

            var resourceLocation = _resourcesLocations.GetValueOrDefault(sceneName);
            var aoh = Addressables.LoadSceneAsync(resourceLocation, mode);

            GetAssetContainer(sceneName).Add(owner, aoh);

            await aoh.Task;

            return aoh.IsValid() ? aoh.Result : default;
        }

        async Task<T> IResourcesService.InstantiateAsync<T>(object owner, string assetName, ResourceData data, InstantiationParameters parameters)
        {
            var resourceLocation = _resourcesLocations.GetValueOrDefault(assetName);
            
            AsyncOperationHandle<GameObject> aoh = default;

            try
            {
                aoh = Addressables.InstantiateAsync(resourceLocation, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ResourcesService)}] Failed instantiate by reference: {e.Message}");
            }

            GetAssetContainer(assetName).Add(owner, aoh);

            await aoh.Task;

            ProcessData(data, aoh.Result);

            var result = aoh.Result;

            _instanceKeys[result] = assetName;

            if (typeof(T) == typeof(GameObject))
            {
                return result as T;
            }

            try
            {
                var component = result.GetComponent<T>();
                return component ?? result as T;
            }
            catch (Exception)
            {
                return result as T;
            }
        }

        async Task<T> IResourcesService.InstantiateAsync<T>(object owner,
                                                            AssetReference reference,
                                                            ResourceData data,
                                                            InstantiationParameters parameters)
        {
            AsyncOperationHandle<GameObject> aoh = default;

            try
            {
                aoh = Addressables.InstantiateAsync(reference, parameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(ResourcesService)}] Failed instantiate by reference: {e.Message}");
            }

            GetAssetContainer(reference.AssetGUID).Add(owner, aoh);

            await aoh.Task;

            ProcessData(data, aoh.Result);

            var result = aoh.Result;

            _instanceKeys[result] = reference.AssetGUID;

            if (typeof(T) == typeof(GameObject))
            {
                return result as T;
            }

            try
            {
                var component = result.GetComponent<T>();
                return component ?? result as T;
            }
            catch (Exception)
            {
                return result as T;
            }
        }

        void IResourcesService.Release(object owner)
        {
            foreach (var container in _containers.Values)
            {
                container.Release(owner);
            }
        }

        void IResourcesService.Release(object owner, string assetName)
        {
            GetAssetContainer(assetName, false)?.Release(owner);
        }
        
        void IResourcesService.Release(object owner, AssetReference assetReference)
        {
            GetAssetContainer(assetReference.AssetGUID, false)?.Release(owner);
        }

        void IResourcesService.ReleaseScene(object owner, string primaryKey, SceneInstance sceneInstance)
        {
            if (string.IsNullOrEmpty(primaryKey))
            {
                _logger.Error($"[{nameof(ResourcesService)}] [{nameof(IResourcesService.Release)}] Argument is null {nameof(primaryKey)} = {primaryKey}");
                return;
            }

            Addressables.UnloadSceneAsync(sceneInstance);

            GetAssetContainer(primaryKey, false)?.Release(owner);
        }

        void IResourcesService.ReleaseInstance(object owner, GameObject instance)
        {
            if (instance != null && _instanceKeys.TryGetValue(instance, out var primaryKey))
            {
                GetAssetContainer(primaryKey).Release(owner, once: true);

                Addressables.ReleaseInstance(instance);

                _instanceKeys.Remove(instance);
            }
            else
            {
                _logger.Error($"[{nameof(ResourcesService)}] PrimaryKey not found for {instance}, owner: {owner}");
            }
        }

        void IResourcesService.ReleaseNullOwners()
        {
            foreach (var asyncOperationsContainer in _containers.Values)
            {
                asyncOperationsContainer.Release(null);
            }
        }

        void IResourcesService.ReleaseAll()
        {
            foreach (var asyncOperationsContainer in _containers.Values)
            {
                asyncOperationsContainer.ReleaseAll();
            }
        }

        private async Task<T> LoadAsync<T>(object owner, string primaryKey)
        {
            var resourceLocation = _resourcesLocations.GetValueOrDefault(primaryKey);

            var aoh = Addressables.LoadAssetAsync<T>(resourceLocation);

            GetAssetContainer(primaryKey).Add(owner, aoh);

            await aoh.Task;

            return aoh.Result;
        }

        private void ProcessData(ResourceData data, Object gameObject)
        {
            if (data.DontDestroy)
            {
                Object.DontDestroyOnLoad(gameObject);
            }

            if (!string.IsNullOrEmpty(data.Name))
            {
                gameObject.name = data.Name;
            }
        }

        private AsyncOperationsContainer GetAssetContainer(string primaryKey, bool createIfMissing = true)
        {
            if (_containers.TryGetValue(primaryKey, out var container))
            {
                return container;
            }

            if (createIfMissing == false)
            {
                return default;
            }

            container = new AsyncOperationsContainer(primaryKey, _logger);

            _containers[primaryKey] = container;

            return container;
        }
    }
}