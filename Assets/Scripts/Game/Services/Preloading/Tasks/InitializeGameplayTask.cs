using Grace.Extend;
using Runner.Core.DI;
using Runner.Core.Services.Tasks;
using Runner.Game.Services.Gameplay.Interfaces;
using UnityEngine;

namespace Runner.Game.Services.Preloading.Tasks
{
    /// <summary>
    /// Задача для инициализации геймплейных сервисов
    /// </summary>
    public sealed class InitializeGameplayTask : TaskBase
    {
        private readonly IContainerFactory _containerFactory;

        public InitializeGameplayTask(IContainerFactory containerFactory, int progressValue = 0)
            : base(progressValue)
        {
            _containerFactory = containerFactory;
        }

        public override async void Activate()
        {
            var gameContainer = (IDependencyContainer)_containerFactory.GetContainer<IGame>();
            var gameplayContainer = _containerFactory.CreateContainer<IGamePlayContainer>(new GamePlayExportsCollection(), true);

            gameplayContainer.CombineWith(gameContainer, true);

            gameContainer.Add(block =>
            {
                //получение IGamePlayContainer через контейнер
                block.ExportFactory<IContainerFactory, IGamePlayContainer>(factory => factory.GetContainer<IGamePlayContainer>());
                
                //задание фабрик методов для инициализации сервисов
                block.ExportFactory<IGamePlayContainer, IPlayerService>(container => container.Locate<IPlayerService>());
                block.ExportFactory<IGamePlayContainer, ITokenService>(container => container.Locate<ITokenService>());
            });

            var allInitialized = await gameplayContainer.InitializeAll<IGameplayService>();
            if (allInitialized)
            {
                SetCompleted();
            }
            else
            {
                Debug.LogError($"[{nameof(InitializeGameplayTask)}] Failed initialize gameplay services");
            }
        }
    }
}