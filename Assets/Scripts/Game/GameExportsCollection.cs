using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Extend;
using Runner.Game.Services;
using Runner.Game.Services.UI;

namespace Runner.Game
{
    public sealed class GameExportsCollection : ExportsCollectionBase
    {
        protected override Type[] ExportedTypes => new[]
        {
            typeof(GameServiceAbstract),
        };

        public override IReadOnlyList<Type> GetExportedTypes()
        {
            var exportedTypes = Assembly.GetExecutingAssembly().ExportedTypes;
            var exports = exportedTypes
                .Where(CanExportType)
                .ToList();

            exports.Add(typeof(UIFactory));
            
            return exports;
        }
    }
}