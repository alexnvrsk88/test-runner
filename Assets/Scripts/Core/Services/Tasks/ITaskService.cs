namespace Runner.Core.Services.Tasks
{
    public interface ITaskService
    {
        void AddTaskQueue(ITasksQueue queue);
    }
}