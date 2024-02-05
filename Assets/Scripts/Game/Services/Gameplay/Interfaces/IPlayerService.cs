using Runner.Game.Models.Player;

namespace Runner.Game.Services.Gameplay.Interfaces
{
    public interface IPlayerService : IGameplayService
    {
        IPlayerModel GetPlayerModel();
    }
}