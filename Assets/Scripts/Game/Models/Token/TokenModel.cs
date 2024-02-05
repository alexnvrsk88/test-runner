using System.Collections.Generic;
using System.Linq;
using Runner.Shared.Shared.Config;

namespace Runner.Game.Models.Token
{
    public sealed class TokenModel : ITokenModel
    {
        string ITokenModel.PrefabName => _tokenConfig.PrefabName;
        IReadOnlyList<ITokenEffectModel> ITokenModel.Effects => _effects;
        
        private readonly TokenConfig _tokenConfig;
        private readonly List<ITokenEffectModel> _effects;

        public TokenModel(TokenConfig tokenConfig)
        {
            _tokenConfig = tokenConfig;
            _effects = new List<ITokenEffectModel>(_tokenConfig.ParameterEffects.Select(c => new TokenEffectModel(c, _tokenConfig.EffectTime)));
        }
    }
}