using System;

namespace Runner.Core.Services.Tasks
{
    public abstract class TaskBase : ITask
    {
        public int ProgressValue { get; }
        public event Action<float> ProgressChanged; 

        protected TaskBase(int progressValue = 0)
        {
            ProgressValue = progressValue;
        }

        protected void SetProgress(float progress)
        {
            ProgressChanged?.Invoke(Math.Clamp(progress, 0f, 1f));
        }
        
        public bool Completed { get; private set; }

        public virtual void Activate()
        {
        }

        public virtual void Deactivate()
        {
            ProgressChanged?.Invoke(1f);
        }

        public virtual void Update()
        {
        }

        protected void SetCompleted()
        {
            Completed = true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
