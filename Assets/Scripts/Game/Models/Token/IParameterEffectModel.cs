using Runner.Shared.Shared;

namespace Runner.Game.Models.Token
{
    /// <summary>
    /// Модель эффекта на параметр игрока
    /// </summary>
    public interface ITokenEffectModel
    {
        float EffectTime { get; }
        PlayerParameter Parameter { get; }
        float Value { get; }
        bool IsMultiplier { get; }
        bool IsAlive { get; }
        
        void UpdateEffectTime(float deltaTime);
    }
}