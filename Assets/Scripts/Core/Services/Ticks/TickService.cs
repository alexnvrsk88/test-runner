using System.Collections.Generic;
using System.Threading.Tasks;
using Grace.Extend;
using Runner.Core.Utils;
using UnityEngine;

namespace Runner.Core.Services.Ticks
{
    [Injection(true, typeof(ITickService))]
    public sealed class TickService : ServiceAbstract, ITickService
    {
        private readonly List<ITickable> _tickables = new();
        private ITickable[] _list;
        private bool _isDirty;

        public override Task<bool> Initialize()
        {
            PlayerLoopUtility.AddToPlayerLoop<TickSystemUpdate>(PlayerLoopLayer.PreUpdate, OnTick);
            return Task.FromResult(true);
        }

        public override void Dispose()
        {
            _tickables.Clear();
            PlayerLoopUtility.ResetPlayerLoop();
        }

        private void OnTick()
        {
            var list = GetList();
            if (_list == null) return;

            for (var i = 0; i < list.Length; i++)
            {
                list[i].OnTick(Time.deltaTime);
            }
        }

        private ITickable[] GetList()
        {
            if (!_isDirty) return _list;
            _isDirty = false;
            _list = _tickables.ToArray();
            return _list;
        }

        void ITickService.Subscribe(ITickable tickable)
        {
            _tickables.Add(tickable);
            _isDirty = true;
        }

        void ITickService.Unsubscribe(ITickable tickable)
        {
            _tickables.Remove(tickable);
            _isDirty = true;
        }

        private struct TickSystemUpdate { }
    }
}