using System.Linq;
using System.Threading.Tasks;
using Grace.Extend;
using Runner.Game.Models.Token;
using Runner.Game.Services.Config;
using Runner.Game.Services.Gameplay.Interfaces;
using Runner.Shared.Shared.Config;
using UnityEngine;

namespace Runner.Game.Services.Gameplay
{
    /// <summary>
    /// Сервис отвечает за создание моделей монет
    /// </summary>
    [Injection(true, typeof(ITokenService))]
    public sealed class TokenService : ITokenService
    {
        private readonly WorldConfig _worldConfig;

        public TokenService(IConfigService configService)
        {
            _worldConfig = configService.GameConfig.WorldConfig;
        }

        public Task<bool> Initialize()
        {
            return Task.FromResult(true);
        }

        ITokenModel ITokenService.GetNextToken()
        {
            var tokenConfig = GetRandomTokenConfig();
            var tokenModel = new TokenModel(tokenConfig);

            return tokenModel;
        }

        private TokenConfig GetRandomTokenConfig()
        {
            var totalWeight = _worldConfig.TokenConfigs.Sum(t => t.SpawnWeight);
            var weight = Random.Range(0, totalWeight);

            foreach (var tokenConfig in _worldConfig.TokenConfigs)
            {
                if (weight <= tokenConfig.SpawnWeight)
                {
                    return tokenConfig;
                }

                weight -= tokenConfig.SpawnWeight;
            }

            return _worldConfig.TokenConfigs[0];
        }
    }
}