using System;
using System.Linq;
using System.Threading.Tasks;
using Plugins.AssetsReference;
using Runner.Core.Services.Resource;
using Runner.Core.Utils;
using UnityEngine.SceneManagement;

namespace Runner.Game.Services.Scenes
{
    /// <summary>
    /// Абстрактный класс контроллера сцены.
    /// Грузит сцену через ресурс сервис и инициализирует её
    /// </summary>
    public abstract class SceneControllerAbstract : ISceneController
    {
        public abstract SceneName SceneName { get; }
        public bool IsActive { get; private set; }
        public bool IsLoaded { get; private set; }

        protected IResourcesService ResourcesService { get; }
        protected IUnityComponent Component;

        protected SceneControllerAbstract(IResourcesService resourcesService)
        {
            ResourcesService = resourcesService;
        }

        //You MUST get references from viewReference on this method 
        protected abstract Task<bool> OnComponentInitialize();
        
        //Triggered after OnComponentInitialize
        protected virtual Task<bool> OnInitialized()
        {
            return Task.FromResult(true);
        }
        
        protected virtual void OnReleased() { }
        
        async Task<ISceneController> ISceneController.Initialize(Action<Scene> callbackLoaded)
        {
            IsActive = true;

            var sceneInstance = await ResourcesService.LoadSceneAsync(this, SceneName.ToString());
            
            Component = sceneInstance.Scene.GetRootGameObjects().Select(rootGameObject => rootGameObject.GetComponent<IUnityComponent>()).FirstOrDefault(ctrl => ctrl != null);
            
            await OnComponentInitialize();
            await OnInitialized();
            
            callbackLoaded.SafeInvoke(sceneInstance.Scene);
            
            return this;
        }

        void ISceneController.Release()
        {
            IsActive = false;
            IsLoaded = false;
            
            OnReleased();

            Component = null;
            
            ResourcesService.Release(this);
        }
    }
}