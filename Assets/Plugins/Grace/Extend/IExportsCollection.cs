using System;
using System.Collections.Generic;

namespace Grace.Extend
{
    public interface IExportsCollection
    {
        IReadOnlyList<Type> GetExportedTypes();
    }
}