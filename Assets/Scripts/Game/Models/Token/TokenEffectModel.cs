using Runner.Shared.Shared;
using Runner.Shared.Shared.Config;

namespace Runner.Game.Models.Token
{
    public sealed class TokenEffectModel : ITokenEffectModel
    {
        public float EffectTime { get; private set; }
        public PlayerParameter Parameter => _parameterEffectConfig.Parameter;
        public float Value => _parameterEffectConfig.Value;
        public bool IsMultiplier => _parameterEffectConfig.IsMultiplier;
        public bool IsAlive => EffectTime > 0f;

        private readonly PlayerParameterEffectConfig _parameterEffectConfig;

        public TokenEffectModel(PlayerParameterEffectConfig parameterEffectConfig, float effectTime)
        {
            EffectTime = effectTime;
            _parameterEffectConfig = parameterEffectConfig;
        }

        void ITokenEffectModel.UpdateEffectTime(float deltaTime)
        {
            EffectTime -= deltaTime;
        }
    }
}