namespace Runner.Core.Services.Ticks
{
    public interface ITickable
    {
        void OnTick(float deltaTime);
    }
}