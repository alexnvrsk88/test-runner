using System;
using Grace.DependencyInjection;

namespace Runner.Core.DI
{
    public interface IGame
    {
        void Inject(object instance, object extraData = null);
        T Locate<T>();
        T Locate<T>(object data = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
        object Locate(Type type);
        object Locate(Type type, object data = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
    }
}