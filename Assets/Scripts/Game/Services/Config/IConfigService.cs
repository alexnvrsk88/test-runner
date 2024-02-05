using Runner.Shared.Shared.Config;

namespace Runner.Game.Services.Config
{
    public interface IConfigService
    {
        GameConfig GameConfig { get; }
    }
}