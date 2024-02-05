using System;
using Runner.Core.Services.Tasks;

namespace Runner.Game.Services.Preloading
{
    public interface IPreloadingService : IGameService
    {
        event Action<float> OnProgressChanged;
        float Progress { get; }
        void StartTasksQueue<T>() where T : ITasksQueue;
    }
}