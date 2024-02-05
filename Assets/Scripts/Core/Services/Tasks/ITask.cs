using System;

namespace Runner.Core.Services.Tasks
{
    public interface ITask
    {
        int ProgressValue { get; }
        event Action<float> ProgressChanged;
        bool Completed { get; }
        void Activate();
        void Deactivate();
        void Update();
    }
}