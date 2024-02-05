using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grace.Extend;
using Runner.Game.Services;

namespace Runner.Game
{
    public sealed class GamePlayExportsCollection : IExportsCollection
    {
        public IReadOnlyList<Type> GetExportedTypes()
        {
            var gameplaySystemAbstractType = typeof(IGameplayService);
            var exports = Assembly.GetExecutingAssembly()
                                  .ExportedTypes
                                  .Where(t => t != gameplaySystemAbstractType && gameplaySystemAbstractType.IsAssignableFrom(t) && !t.IsAbstract)
                                  .ToList();

            return exports;
        }
    }
}