using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Runner.Core.Services.Resource
{
    public interface IResourcesService
    {
        Task<T> LoadAsync<T>(object owner, string assetName);
        Task<T> LoadAsync<T>(object owner, AssetReference assetReference);
        Task<SceneInstance> LoadSceneAsync(object owner, string sceneName, LoadSceneMode mode = LoadSceneMode.Single);
        Task<T> InstantiateAsync<T>(object owner, string assetName, ResourceData data = default, InstantiationParameters parameters = default) where T : class;
        Task<T> InstantiateAsync<T>(object owner, AssetReference reference, ResourceData data = default, InstantiationParameters parameters = default) where T : class;
        void Release(object owner);
        void Release(object owner, string assetName);
        void Release(object owner, AssetReference assetReference);
        void ReleaseScene(object owner, string sceneName, SceneInstance sceneInstance);
        void ReleaseInstance(object owner, GameObject instancedObject);
        void ReleaseNullOwners();
        void ReleaseAll();
    }
}