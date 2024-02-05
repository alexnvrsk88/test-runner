using System.Threading.Tasks;

namespace Runner.Game.Services
{
    public abstract class GameServiceAbstract : IGameService
    {
        public abstract Task<bool> Initialize();
    }
}