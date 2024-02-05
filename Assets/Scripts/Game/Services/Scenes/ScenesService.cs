using System;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Extend;
using UnityEngine.SceneManagement;

namespace Runner.Game.Services.Scenes
{
    /// <summary>
    /// Сервис отвечает за переключение сцен 
    /// </summary>
    [Injection(true, typeof(IScenesService))]
    public sealed class ScenesService : GameServiceAbstract, IScenesService
    {
        public event Action<ISceneController> OnSceneControllerChanged;
        
        private ISceneController _activeSceneController;
        private readonly IExportLocatorScope _container;
        
        public ScenesService(IExportLocatorScope container)
        {
            _container = container;
        }
        
        public override Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        async void IScenesService.SwitchTo<TSceneController>(Action<Scene> callbackLoaded)
        {
            ReleaseActiveController();
            
            var controller = await _container.Locate<TSceneController>().Initialize(callbackLoaded);
            _activeSceneController = controller;
            
            OnSceneControllerChanged?.Invoke(_activeSceneController);
        }

        TController IScenesService.GetActiveSceneController<TController>()
        {
            if (_activeSceneController is not TController controller)
            {
                throw new ArgumentException($"Active scene controller is not {typeof(TController).Name}");
            }
                    
            return controller;
        }
        
        ISceneController IScenesService.GetActiveSceneController()
        {
            return _activeSceneController;
        }

        private void ReleaseActiveController()
        {
            _activeSceneController?.Release();
        }
    }
}
