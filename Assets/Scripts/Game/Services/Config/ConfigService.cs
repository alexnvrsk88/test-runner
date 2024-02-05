using System.Threading.Tasks;
using Grace.Extend;
using Runner.Core.Services.Resource;
using Runner.Shared.Shared.Config;

namespace Runner.Game.Services.Config
{
    /// <summary>
    /// Сервис отчечает за конфиг игры.
    /// </summary>
    [Injection(true, typeof(IConfigService))]
    public sealed class ConfigService : GameServiceAbstract, IConfigService
    {
        private readonly IResourcesService _resourcesService;

        public GameConfig GameConfig { get; private set; }

        public ConfigService(IResourcesService resourcesService)
        {
            _resourcesService = resourcesService;
        }

        public override async Task<bool> Initialize()
        {
            GameConfig = await _resourcesService.LoadAsync<GameConfig>(this, nameof(GameConfig));
            
            return true;
        }
    }
}