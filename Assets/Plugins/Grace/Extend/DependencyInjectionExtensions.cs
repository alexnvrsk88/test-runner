using System;
using System.Reflection;
using Grace.DependencyInjection;

namespace Grace.Extend
{
    public static class DependencyInjectionExtensions
    {
        public static IDependencyContainer Configure(this IDependencyContainer currentContainer, IExportsCollection exports)
        {
            var container = ((DependencyContainer)currentContainer);
            container.AddExports(exports);
            container.Add(block =>
            {
                foreach (var type in exports.GetExportedTypes())
                {
                    if (type.IsAbstract || type.IsGenericType) continue;
                    
                    var configuration = block.Export(type);
                    
                    if (GetAttribute(type, out var attribute))
                    {
                        if (attribute.IsSingleton)
                        {
                            configuration.Lifestyle.Singleton();
                        }

                        if (attribute.InterfaceType != null)
                        {
                            configuration.As(attribute.InterfaceType);
                        }
                        else
                        {
                            if (type.GetInterfaces().Length > 0)
                            {
                                configuration.ByInterfaces();
                            }
                        }
                    }
                    else
                    {
                        if (type.GetInterfaces().Length > 0)
                        {
                            configuration.ByInterfaces();
                        }
                    }
                }
            });
            return currentContainer;
        }

        public static IDependencyContainer Configure(this IDependencyContainer currentContainer, IConfigurationModule configurationModule)
        {
            var container = ((DependencyContainer)currentContainer);
            container.Add(configurationModule);
            return currentContainer;
        }

        private static bool GetAttribute(ICustomAttributeProvider type, out InjectionAttribute foundAttribute)
        {
            foreach (Attribute attribute in type.GetCustomAttributes(false))
            {
                if (attribute is InjectionAttribute injectionAttribute)
                {
                    foundAttribute = injectionAttribute;
                    return true;
                }
            }

            foundAttribute = null;
            return false;
        }
    }
}