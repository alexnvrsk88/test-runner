using Grace.DependencyInjection;

namespace Grace.Extend
{
    public interface IContainerFactory
    {
        IDependencyContainer CreateContainer<TContainerType>(IExportsCollection exportsCollection, bool addSelf = false, params IConfigurationModule[] configurationModules);
        T GetContainer<T>();
        void DisposeContainer<T>();
    }
}