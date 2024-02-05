namespace Runner.Core.Services.Ticks
{
    public interface ITickService
    {
        void Subscribe(ITickable tickable);
        void Unsubscribe(ITickable tickable);
    }
}