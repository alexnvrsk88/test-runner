using Runner.Core.Services.Tasks;
using Runner.Game.Services.UI.Interfaces;
using Runner.Game.Services.UI.Params;

namespace Runner.Game.Services.Preloading.Tasks
{
    /// <summary>
    /// Задача для показа UI окна
    /// </summary>
    /// <typeparam name="TViewController"></typeparam>
    public class ShowViewTask<TViewController> : TaskBase where TViewController : class, IViewController
    {
        private readonly IUIService _uiSystem;
        
        public ShowViewTask(IUIService uiSystem, int progress) : base(progress)
        {
            _uiSystem = uiSystem;
        }

        public override async void Activate()
        {
            await _uiSystem.Show<TViewController>();
            SetCompleted();
        }
    }
    
    /// <summary>
    /// Задача для показа UI окна с параметрами
    /// </summary>
    /// <typeparam name="TViewController"></typeparam>
    /// <typeparam name="TParams"></typeparam>
    public class ShowViewTask<TViewController, TParams> : TaskBase where TViewController : class, IViewController where TParams : ControllerParams, new()
    {
        private readonly IUIService _uiSystem;
        
        public ShowViewTask(IUIService uiSystem, int progress) : base(progress)
        {
            _uiSystem = uiSystem;
        }

        public override async void Activate()
        {
            await _uiSystem.Show<TViewController>(new TParams());
            SetCompleted();
        }
    }
}