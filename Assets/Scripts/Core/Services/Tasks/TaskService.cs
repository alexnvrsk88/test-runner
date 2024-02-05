using System.Collections.Generic;
using System.Threading.Tasks;
using Grace.Extend;
using Runner.Core.Services.Logging;
using Runner.Core.Utils;

namespace Runner.Core.Services.Tasks
{
    /// <summary>
    /// Сервис обработки очередей задач
    /// </summary>
    [Injection(true, typeof(ITaskService))]
    public sealed class TaskService : ServiceAbstract, ITaskService
    {
        private readonly List<ITasksQueue> _tasksQueues = new List<ITasksQueue>();
        private readonly List<ITasksQueue> _finishedQueues = new List<ITasksQueue>();

        private readonly ILoggingService _logger;

        public TaskService(ILoggingService logger)
        {
            _logger = logger;
        }

        public override Task<bool> Initialize()
        {
            PlayerLoopUtility.AddToPlayerLoop<TaskSystemUpdate>(PlayerLoopLayer.PreUpdate, Update);
            return Task.FromResult(true);
        }

        private void Update()
        {
            _finishedQueues.Clear();
            
            foreach (var taskQueue in _tasksQueues)
            {
                taskQueue.Update();
                
                if (taskQueue.Completed)
                {
                    _finishedQueues.Add(taskQueue);
                }
            }

            foreach (var finishedQueue in _finishedQueues)
            {
                _tasksQueues.Remove(finishedQueue);
            }
        }

        void ITaskService.AddTaskQueue(ITasksQueue queue)
        {
            _tasksQueues.Add(queue);
        }

        private struct TaskSystemUpdate { }
    }
}