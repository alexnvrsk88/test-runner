using System;
using System.Collections.Generic;
using System.Linq;

namespace Grace.Extend
{
    public abstract class ExportsCollectionBase : IExportsCollection
    {
        protected abstract Type[] ExportedTypes { get; }

        public abstract IReadOnlyList<Type> GetExportedTypes();

        protected bool CanExportType(Type type)
        {
            return type.IsAbstract == false && ExportedTypes.Any(t => IsSubclassOf(type, t));
        }

        private bool IsSubclassOf(Type type, Type abstractType)
        {
            return type != abstractType && abstractType.IsAssignableFrom(type);
        }
    }
}