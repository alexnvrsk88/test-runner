using Grace.DependencyInjection;
using Runner.Core.Services.Logging;
using Runner.Core.Services.Tasks;
using Runner.Game.Services.Preloading.Tasks;
using Runner.Game.Services.Scenes.Impl;
using Runner.Game.UI.Loading;

namespace Runner.Game.Services.Preloading.Queues
{
    /// <summary>
    /// Основная очередь загрузки приложения
    /// </summary>
    public sealed class PreloaderTasksQueue : TasksQueue<TaskBase>
    {
        public PreloaderTasksQueue(ILocatorService container, ILoggingService loggingService) : base(loggingService)
        {
            Tasks.Enqueue(container.Locate<ShowViewTask<LoadingView>>(0)); // показываем окно лоадера
            Tasks.Enqueue(container.Locate<InitializeGameplayTask>(50)); // иницаилизируем геймлейные сервисы
            Tasks.Enqueue(container.Locate<SwitchSceneTask<GameSceneController>>(90)); // грузим сцену и инициализируем её
            Tasks.Enqueue(container.Locate<WaitForTime>(new { finishTime = .5f, progressValue = 99 })); // ждем для отображения прогресса
            Tasks.Enqueue(container.Locate<CloseViewTask<LoadingView>>(100)); // закрываем окно лоадера
        }
    }
}