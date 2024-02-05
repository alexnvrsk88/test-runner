using Runner.Core.Services.Tasks;
using UnityEngine;

namespace Runner.Game.Services.Preloading.Tasks
{
    /// <summary>
    /// Задача для ожидания по времени
    /// </summary>
    public sealed class WaitForTime : TaskBase
    {
        private readonly float _finishTime;
        private float _timer;
        
        public WaitForTime(float finishTime, int progressValue) : base(progressValue)
        {
            _finishTime = finishTime;
        }

        public override void Activate()
        {
            _timer = 0f;
        }

        public override void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= _finishTime)
            {
                SetCompleted();
            }
        }
    }
}