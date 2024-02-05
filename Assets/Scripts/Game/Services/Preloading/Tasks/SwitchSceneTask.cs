using Runner.Core.Services.Tasks;
using Runner.Game.Services.Scenes;
using UnityEngine.SceneManagement;

namespace Runner.Game.Services.Preloading.Tasks
{
    /// <summary>
    /// Задача переключения сцены
    /// </summary>
    /// <typeparam name="TSceneController"></typeparam>
    public sealed class SwitchSceneTask<TSceneController> : TaskBase where TSceneController : ISceneController
    {
        private readonly IScenesService _scenesService;

        public SwitchSceneTask(IScenesService scenesService, int progressValue) : base(progressValue)
        {
            _scenesService = scenesService;
        }

        public override void Activate()
        {
            _scenesService.SwitchTo<TSceneController>(SceneLoadedHandler);
        }

        private void SceneLoadedHandler(Scene scene)
        {
            SetCompleted();
        }
    }
}