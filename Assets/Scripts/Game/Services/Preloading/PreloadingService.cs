using System;
using System.Threading.Tasks;
using Grace.DependencyInjection;
using Grace.Extend;
using Runner.Core.Services.Logging;
using Runner.Core.Services.Tasks;
using Runner.Core.Utils;
using Runner.Game.Services.UI.Interfaces;
using UnityEngine;

namespace Runner.Game.Services.Preloading
{
    /// <summary>
    /// Сервис для запуска очереди задач.
    /// Запускаети очередь задач и следит за её прогрессом
    /// </summary>
    [Injection(true, typeof(IPreloadingService))]
    public sealed class PreloadingService : GameServiceAbstract, IPreloadingService
    {
        private readonly ILoggingService _loggingService;
        private readonly IExportLocatorScope _container;
        private readonly ITaskService _taskService;
        private readonly IUIService _uiSystem;
        private ITasksQueue _tasksQueue;

        public event Action<float> OnProgressChanged;
        public float Progress { get; private set; }
        public bool IsCompleted { get; private set; }

        public PreloadingService(IExportLocatorScope container, ITaskService taskService)
        {
            _container = container;
            _taskService = taskService;
        }

        public override Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        public void StartTasksQueue<T>() where T : ITasksQueue
        {
            Progress = 0f;
            IsCompleted = false;
            
            _tasksQueue = _container.Locate<T>();
            _tasksQueue.OnProgressChanged += ProgressChangedHandler;
            _tasksQueue.OnCompleted += CompletedHandler;
            
            _taskService.AddTaskQueue(_tasksQueue);
            
            _tasksQueue.Start();
        }

        private void ProgressChangedHandler(float progress)
        {
            Debug.Log($"[{nameof(PreloadingService)}] progress changed - {progress:F}");
            Progress = progress;
            OnProgressChanged.SafeInvoke(Progress);
        }

        private void CompletedHandler()
        {
            Progress = 1f;
            IsCompleted = true;
            _tasksQueue.OnProgressChanged -= ProgressChangedHandler;
            _tasksQueue.OnCompleted -= CompletedHandler;
        }
    }
}