using System.Threading.Tasks;
using Grace.Extend;
using Runner.Game.Models.Player;
using Runner.Game.Services.Config;
using Runner.Game.Services.Gameplay.Interfaces;
using Runner.Shared.Shared.Config;

namespace Runner.Game.Services.Gameplay
{
    /// <summary>
    /// Сервис отвечает за работу с моделью игрока
    /// </summary>
    [Injection(true, typeof(IPlayerService))]
    public sealed class PlayerService : IPlayerService
    {
        private readonly PlayerConfig _playerConfig;
        
        private IPlayerModel _playerModel;

        public PlayerService(IConfigService configService)
        {
            _playerConfig = configService.GameConfig.PlayerConfig;
        }

        public Task<bool> Initialize()
        {
            _playerModel = new PlayerModel(_playerConfig);
            return Task.FromResult(true);
        }

        IPlayerModel IPlayerService.GetPlayerModel()
        {
            return _playerModel;
        }
    }
}