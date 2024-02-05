using Runner.Shared.Shared;

namespace Runner.Game.Models.Player
{
    /// <summary>
    /// Модель игрока, содержащая базовые параметры игрока и его визуал (префаб) 
    /// </summary>
    public interface IPlayerModel
    {
        string PlayerPrefabName { get; }
        float GetBaseParameterValue(PlayerParameter playerParameter);
    }
}