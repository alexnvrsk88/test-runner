using System;

namespace Runner.Core.Services.Tasks
{
    /// <summary>
    /// Интерфейс для очереди задач, позволяет запускать очереди и следить за её состоянием
    /// </summary>
    public interface ITasksQueue 
    {
        event Action<float> OnProgressChanged;
        event Action OnCompleted;
        bool Completed { get; }
        void Start();
        void Update();
    }
}