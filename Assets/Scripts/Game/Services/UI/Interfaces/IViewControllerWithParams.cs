using Runner.Game.Services.UI.Params;

namespace Runner.Game.Services.UI.Interfaces
{
    public interface IViewControllerWithParams : IViewController
    {
        void UpdateParams(IControllerParams controllerParams);
    }
}