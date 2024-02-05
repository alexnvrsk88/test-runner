using System;
using System.Collections.Generic;
using Runner.Core.Utils;
using Runner.Core.Services.Logging;
using UnityEngine;

namespace Runner.Core.Services.Tasks
{
    public class TasksQueue<T> : IDisposable, ITasksQueue where T : ITask
    {
        private const float PERCENT_MULTIPLIER = .01f;
        private const int MAX_PROGRESS = 100;
        
        protected readonly Queue<T> Tasks;

        private T _currentTask;
        private int _currentStepIndex;
        private int _loadingProgress;
        private int _lastTaskLoadingProgress;
        public event Action<float> OnProgressChanged;
        public event Action OnCompleted;

        public bool Completed { get; private set; }
        private ILoggingService LoggingService { get; }

        protected TasksQueue(ILoggingService loggingService)
        {
            LoggingService = loggingService;
            Tasks = new Queue<T>();
        }

        public virtual void Start()
        {
            QueueStarted();

            PlayNextStep();
        }

        public virtual void Update()
        {
            if (_currentTask == null)
            {
                return;
            }

            try
            {
                _currentTask.Update();
            }
            catch (Exception e)
            {
                HandleError($"Error update {_currentTask}: {e}");
            }

            if (_currentTask.Completed)
            {
                PlayNextStep();
            }
        }

        public virtual void Dispose()
        {
            _currentTask?.Deactivate();
            _currentTask = default;

            OnProgressChanged = null;
        }

        protected virtual void OnTaskStarted() { }

        protected virtual void OnTaskCompleted() { }

        protected virtual void OnQueueStarted() { }

        protected virtual void OnQueueCompleted() { }

        private void PlayNextStep()
        {
            if (_currentTask != null)
            {
                try
                {
                    _currentTask.Deactivate();
                }
                catch (Exception e)
                {
                    HandleError($"Error deactivate {_currentTask}: {e}");
                }

                TaskCompleted();
            }

            if (Tasks.Count > 0)
            {
                _currentTask = Tasks.Dequeue();

                try
                {
                    _currentTask.Activate();
                }
                catch (Exception e)
                {
                    HandleError($"Error activate {_currentTask}: {e}");
                }

                _currentStepIndex++;

                TaskStarted();
            }
            else
            {
                Completed = true;

                Dispose();

                QueueCompleted();
            }
        }

        private void TaskStarted()
        {
            LoggingService.Info($"[{Time.frameCount}] [{GetType().Name}] Task started: {_currentTask} ({_currentStepIndex}/{Tasks.Count + _currentStepIndex})");
            
            OnTaskStarted();
        }

        private void TaskCompleted()
        {
            _loadingProgress = _currentTask.ProgressValue;
            _lastTaskLoadingProgress = _loadingProgress;

            _currentTask.ProgressChanged -= HandleTaskProgressChanged;

            LoggingService.Info($"[{Time.frameCount}] [{GetType().Name}] Completed: {_currentTask} ({_currentStepIndex}/{Tasks.Count + _currentStepIndex})");

            OnTaskCompleted();
        }

        private void QueueStarted()
        {
            foreach (var task in Tasks)
            {
                task.ProgressChanged += HandleTaskProgressChanged;
            }

            LoggingService.Info($"[{Time.frameCount}] [{GetType().Name}] Queue started");
            OnQueueStarted();
        }

        private void QueueCompleted()
        {
            OnCompleted?.Invoke();
            LoggingService.Info($"[{Time.frameCount}] [{GetType().Name}] Queue completed");

            OnQueueCompleted();
        }

        private void HandleError(string errorDescription)
        {
            LoggingService.Error($"[{Time.frameCount}] [{GetType().Name}] Queue error: {errorDescription}");
        }

        private void HandleTaskProgressChanged(float taskProgress)
        {
            var taskTotalProgress = _currentTask.ProgressValue - _lastTaskLoadingProgress;
            var totalProgress = _lastTaskLoadingProgress + taskTotalProgress * taskProgress;
            
            totalProgress = MathF.Min(totalProgress, MAX_PROGRESS);

            _loadingProgress = (int) MathF.Round(totalProgress);

            OnProgressChanged.SafeInvoke(_loadingProgress * PERCENT_MULTIPLIER);
        }
    }
}