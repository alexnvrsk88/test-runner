using Grace.DependencyInjection;

namespace Runner.Core.DI
{
    public interface ICore
    {
        T Locate<T>();
        T Locate<T>(object data = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
    }
}