using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Runner.Core.Services.Cache
{
    public interface ICacheProvider
    {
        Type CachedType { get; }
        Task<GameObject> Get(object cached, Transform parent, bool createIfEmpty = true, bool wake = false);
        bool Return(GameObject instance);
        bool ReturnDeferred(GameObject instance, float delay, Action callback = null);
        void Clear(bool forced);
        void ClearCached();
        int GetCachedCount(object cached);
        string LogStatus();
    }
}