using Grace.Extend;
using Runner.Core;
using Runner.Core.DI;
using Runner.Core.Services;
using Runner.Game.Services;
using Runner.Game.Services.Preloading;
using Runner.Game.Services.Preloading.Queues;
using UnityEngine;

namespace Runner.Game
{
    /// <summary>
    /// Основной класс инициализации приложения
    /// </summary>
    public sealed class Initializer : MonoBehaviour
    {
        private readonly ContainerFactory _factory = new();

        private IDependencyContainer _core;
        private IDependencyContainer _game;

        private void Awake()
        {
            Initialize();

            DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            _core?.Dispose();
            _game?.Dispose();
        }

        private async void Initialize()
        {
            // создание Core контейнера
            _core = _factory.CreateContainer<ICore>(new CoreExportsCollection());

            // создание Game контейнера в связке с Core
            _game = _factory.CreateContainer<IGame>(new GameExportsCollection())
                            .CombineWith(_core);

            // инициализация сервисов
            await _core.InitializeAll<IService>();
            await _game.InitializeAll<IGameService>();

            // запуск очереди задач на иницилизацию игры
            if (_game.TryLocate(out IPreloadingService preloadingService))
            {
                preloadingService.StartTasksQueue<PreloaderTasksQueue>();
            }
        }
    }
}