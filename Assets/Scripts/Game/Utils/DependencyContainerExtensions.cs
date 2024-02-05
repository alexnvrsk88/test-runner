using System;
using System.Linq;
using System.Threading.Tasks;
using Grace.Extend;
using Runner.Core.DI;
using UnityEngine.Pool;

namespace Runner.Game
{
    public static class DependencyContainerExtensions
    {
        public static async Task<bool> InitializeAll<T>(this IDependencyContainer container) where T : IInitializable
        {
            var tasksList = ListPool<Task>.Get();
            var exports = ((DependencyContainer)container).CompositeExports
                                                          .Where(t => t.GetInterfaces()
                                                                       .Any(i => i == typeof(T)))
                                                          .GroupBy(GetInitializationOrder)
                                                          .OrderBy(types => types.Key);

            foreach (var export in exports)
            {
                foreach (var classType in export)
                {
                    classType.GetAttribute<InjectionAttribute>(out var attribute);

                    if (attribute?.InterfaceType != null)
                    {
                        if (container.TryLocate(attribute.InterfaceType, out var system))
                        {
                            if (system is T initializable)
                            {
                                tasksList.Add(initializable.Initialize());
                            }
                        }
                    }
                    else
                    {
                        if (container.TryLocate(classType, out var system))
                        {
                            if (system is T initializable)
                            {
                                tasksList.Add(initializable.Initialize());
                            }
                        }
                    }
                }

                await Task.WhenAll(tasksList);

                tasksList.Clear();
            }

            await Task.WhenAll(tasksList);

            ListPool<Task>.Release(tasksList);

            return true;
        }

        public static int GetInitializationOrder(Type systemType)
        {
            var attributes = systemType.GetCustomAttributes(false);

            foreach (Attribute attribute in attributes)
            {
                if (attribute is InitializationAttribute ageAttribute)
                {
                    return ageAttribute.Order;
                }
            }

            return int.MaxValue;
        }
    }
}