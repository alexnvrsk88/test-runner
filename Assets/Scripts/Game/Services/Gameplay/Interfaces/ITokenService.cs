using Runner.Game.Models.Token;

namespace Runner.Game.Services.Gameplay.Interfaces
{
    public interface ITokenService : IGameplayService
    {
        ITokenModel GetNextToken();
    }
}