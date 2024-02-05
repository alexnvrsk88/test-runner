using Runner.Core.Services.Tasks;
using Runner.Game.Services.UI.Interfaces;

namespace Runner.Game.Services.Preloading.Tasks
{
    /// <summary>
    /// Задача для закрытия UI окна
    /// </summary>
    /// <typeparam name="TViewController"></typeparam>
    public sealed class CloseViewTask<TViewController> : TaskBase where TViewController : class, IViewController
    {
        private readonly IUIService _uiSystem;

        public CloseViewTask(IUIService uiSystem, int progress) : base(progress)
        {
            _uiSystem = uiSystem;
        }

        public override void Activate()
        {
            _uiSystem.Close<TViewController>();

            SetCompleted();
        }
    }
}