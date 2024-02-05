using System;
using Grace.DependencyInjection;

namespace Grace.Extend
{
    public interface IDependencyContainer
    {
        void Inject(object instance, object extraData = null);
        void Add(Action<IExportRegistrationBlock> registrationAction);
        bool TryLocate(Type type, out object value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
        bool TryLocate<T>(out T value, object extraData = null, ActivationStrategyFilter consider = null, object withKey = null, bool isDynamic = false);
        IDependencyContainer CombineWith(IDependencyContainer parentContainer, bool parentExports = false);
        void Dispose();
    }
}