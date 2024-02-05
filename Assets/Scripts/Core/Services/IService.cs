using System;
using Runner.Core.DI;

namespace Runner.Core.Services
{
    public interface IService : IDisposable, IInitializable
    {
    }
}
