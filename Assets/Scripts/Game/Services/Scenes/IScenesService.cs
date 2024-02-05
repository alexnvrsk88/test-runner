using System;
using UnityEngine.SceneManagement;

namespace Runner.Game.Services.Scenes
{
    public interface IScenesService
    {
        event Action<ISceneController> OnSceneControllerChanged;
        void SwitchTo<TController>(Action<Scene> callbackLoaded = null) where TController : ISceneController;
        TController GetActiveSceneController<TController>() where TController : ISceneController;
        ISceneController GetActiveSceneController();
    }
}