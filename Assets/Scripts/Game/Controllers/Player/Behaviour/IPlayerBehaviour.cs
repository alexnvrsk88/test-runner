namespace Runner.Game.Controllers.Player.Behaviour
{
    /// <summary>
    /// Интерфейс поведения игрока для управления им
    /// </summary>
    public interface IPlayerBehaviour
    {
        bool IsActive { get; }
        
        void Update(float deltaTime);
        void FixedUpdate(float deltaTime);
        void Activate();
        void Deactivate();
    }
}