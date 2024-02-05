using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Extend;
using Runner.Core.Services;

namespace Runner.Core
{
    public sealed class CoreExportsCollection : ExportsCollectionBase
    {
        protected override Type[] ExportedTypes => new[]
        {
            typeof(ServiceAbstract)
        };

        public override IReadOnlyList<Type> GetExportedTypes()
        {
            var exportedTypes = Assembly.GetExecutingAssembly().ExportedTypes;
            var exports = exportedTypes
                .Where(CanExportType)
                .ToList();
            
            return exports;
        }
    }
}