using Grace.Extend;

namespace Runner.Core.DI
{
    [Injection(false, typeof(IGamePlayContainer))]
    public sealed class GamePlayContainer : DependencyContainer, IGamePlayContainer
    {
    }
}