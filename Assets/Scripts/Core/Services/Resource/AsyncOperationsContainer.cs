using System;
using System.Collections.Generic;
using System.Linq;
using Runner.Core.Services.Logging;
using Runner.Core.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Runner.Core.Services.Resource
{
    /// <summary>
    /// Контейнер для хранения операций загрузки ресурсов
    /// </summary>
    internal class AsyncOperationsContainer
    {
        private readonly List<AsyncOperationHandle> _handlers = new();
        private readonly List<WeakReference<object>> _owners = new();
        private readonly string _primaryKey;
        private readonly ILoggingService _logger;

        public AsyncOperationsContainer(string primaryKey, ILoggingService logger)
        {
            _primaryKey = primaryKey;
            _logger = logger;
        }

        public void Add(object owner, AsyncOperationHandle operation)
        {
            _owners.Add(new WeakReference<object>(owner));
            _handlers.Add(operation);
        }

        public void Release(object owner, bool once = false)
        {
            for (var i = 0; i < _owners.Count; i++)
            {
                var weakReference = _owners[i];
                if (weakReference.TryGetTarget(out var target) && target != owner)
                {
                    continue;
                }

                _owners.RemoveBySwap(i);
                i--;

                if (once)
                {
                    break;
                }
            }

            if (_owners.Count == 0)
            {
                ReleaseAll();
            }
        }

        public void ReleaseAll()
        {
            _owners.Clear();

            if (!_handlers.Any())
            {
                return;
            }

            for (var i = 0; i < _handlers.Count; i++)
            {
                var asyncOperationHandle = _handlers[i];
                if (asyncOperationHandle.IsValid())
                {
                    Addressables.Release(asyncOperationHandle);
                }
            }

            _handlers.Clear();
        }
    }
}