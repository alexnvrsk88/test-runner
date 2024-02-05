using System;
using Grace.DependencyInjection.Impl;
using Grace.Extend;

namespace Runner.Core.DI
{
    public sealed class GameContainer : DependencyContainer, IGame
    {
        private void EnsureUsagesForIL2CPP()
        {
            EnsureUsageForIL2CPP<Int32>();
            EnsureUsageForIL2CPP<bool>();
            EnsureUsageForIL2CPP<Single>();
            throw new InvalidOperationException("Do not call this method!");
        }

        private void EnsureUsageForIL2CPP<T>()
        {
            ScopeConfiguration.Implementation.Locate<IInjectionContextValueProvider>().GetValueFromInjectionContext<T>(null, null, null, null, null, false, false);
        }
    }
}