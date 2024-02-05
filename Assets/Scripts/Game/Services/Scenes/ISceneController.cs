using System;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Runner.Game.Services.Scenes
{
    public interface ISceneController
    {
        bool IsActive { get; }
        bool IsLoaded { get; }
        Task<ISceneController> Initialize(Action<Scene> callbackLoaded);
        void Release();
        SceneName SceneName { get; }
    }
}
