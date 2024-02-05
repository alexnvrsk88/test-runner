using Runner.Shared.Shared;
using Runner.Shared.Shared.Config;

namespace Runner.Game.Models.Player
{
    public sealed class PlayerModel : IPlayerModel
    {
        public string PlayerPrefabName => _playerConfig.PlayerPrefabName;
        
        private readonly PlayerConfig _playerConfig;

        public PlayerModel(PlayerConfig playerConfig)
        {
            _playerConfig = playerConfig;
        }

        public float GetBaseParameterValue(PlayerParameter playerParameter)
        {
            return _playerConfig.GetBaseParameterValue(playerParameter);
        }
    }
}