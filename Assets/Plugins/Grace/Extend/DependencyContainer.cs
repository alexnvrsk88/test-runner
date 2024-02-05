using System;
using System.Collections.Generic;
using Grace.DependencyInjection;

namespace Grace.Extend
{
    public abstract class DependencyContainer : DependencyInjectionContainer, IDependencyContainer
    {
        public IReadOnlyList<Type> NativeExports { get; set; }
        public List<Type> CompositeExports { get; private set; } = new ();
        private List<Type> ParentExports { get; set; } = new ();


        public void AddExports(IExportsCollection exportsCollection)
        {
            NativeExports = exportsCollection.GetExportedTypes();
            CompositeExports.AddRange(NativeExports);
        }

        public void Inject(object instance, object extraData = null)
        {
            ((DependencyInjectionContainer) this).Inject(instance, extraData);
        }

        /// <summary>
        /// Combines exports between containers
        /// </summary>
        /// <param name="parentContainer">Parent container</param>
        /// <param name="parentExports">Parent container can consist of another parent containers exports.
        /// If you want to import all export from all parents, just set this tru</param>
        /// <returns></returns>
        public IDependencyContainer CombineWith(IDependencyContainer parentContainer, bool parentExports = false)
        {
            if (parentContainer == null) return this;
            
            if (parentExports)
            {
                ParentExports.AddRange(((DependencyContainer)parentContainer).CompositeExports);    
            }
            else
            {
                ParentExports.AddRange(((DependencyContainer)parentContainer).NativeExports);
            }
            
            
            Add(block =>
            {
                foreach (var classType in ParentExports)
                {
                    classType.GetAttribute<InjectionAttribute>(out var attribute);
                    
                    if (attribute != null && attribute.HasInterface())
                    {
                        if (attribute.LocateByCombine)
                        {
                            if (parentContainer.TryLocate(attribute.InterfaceType, out var locatedByInterface))
                            {
                                block.ExportInstance(locatedByInterface).As(attribute.InterfaceType).ExternallyOwned();
                            }
                        }
                        else
                        {
                            block.Export(classType).As(attribute.InterfaceType);
                        }
                    }
                    else
                    {
                        if (!classType.IsInterface && !classType.IsGenericType)
                        {
                            block.Export(classType);
                        }  
                    }
                }
            });

            CompositeExports.AddRange(ParentExports);
            return this;
        }
    }
}