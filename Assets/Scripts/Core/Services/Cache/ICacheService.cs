using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Runner.Core.Services.Cache
{
    public interface ICacheService
    {
        Task<GameObject> Get<T>(object key, Transform parent);
        void Return(GameObject gameObject);
        void ReturnDeferred(GameObject gameObject, float delay, Action onReturn = null);
        void ClearAll(bool forced = false);
    }
}