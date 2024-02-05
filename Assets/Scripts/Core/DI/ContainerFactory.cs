using System;
using System.Collections.Generic;
using Grace.DependencyInjection;
using Grace.Extend;

namespace Runner.Core.DI
{
    public sealed class ContainerFactory : IContainerFactory
    {
        private readonly DependencyInjectionContainer _factoryContainer = new ();

        private Dictionary<Type, IDependencyContainer> _containers = new ();

        public ContainerFactory()
        {
            _factoryContainer.Configure(block =>
            {
                block.Export<CoreContainer>().As<ICore>().Lifestyle.Singleton();
                block.Export<GameContainer>().As<IGame>().Lifestyle.Singleton();
                block.Export<GamePlayContainer>().As<IGamePlayContainer>().Lifestyle.Singleton();
                block.Export<ContainerFactory>().As<IContainerFactory>();
            });
        }

        T IContainerFactory.GetContainer<T>()
        {
            _containers.TryGetValue(typeof(T), out var container);
            return (T)container;
        }

        public void DisposeContainer<T>()
        {
            _containers.TryGetValue(typeof(T), out var container);

            if (container == null) return;
            
            container.Dispose();
            _containers.Remove(typeof(T));
        }

        public IDependencyContainer CreateContainer<TContainerType>(IExportsCollection exportsCollection = null,
                                                                    bool addSelf = false,
                                                                    params IConfigurationModule[] configurationModules)
        {
            var dependencyContainer = _factoryContainer.Locate<TContainerType>() as IDependencyContainer;
            
            _containers.Add(typeof(TContainerType), dependencyContainer);
            
            if (exportsCollection != null)
            {
                dependencyContainer.Configure(exportsCollection);
            }

            if (configurationModules != null)
            {
                for (var i = 0; i < configurationModules.Length; i++)
                {
                    var configurationModule = configurationModules[i];
                    dependencyContainer.Configure(configurationModule);
                }
            }

            dependencyContainer.Add(block =>
            {
                block.ExportInstance(this).As<IContainerFactory>();

                if (addSelf)
                {
                    block.ExportInstance(dependencyContainer).As<TContainerType>();
                }
                
            });
            return dependencyContainer;
        }
    }
}