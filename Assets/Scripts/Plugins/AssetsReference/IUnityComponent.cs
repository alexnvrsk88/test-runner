using UnityEngine;
using Object = UnityEngine.Object;

namespace Plugins.AssetsReference
{
    public interface IUnityComponent
    {
        GameObject GameObject { get; }
        Transform Transform { get; }
        RectTransform RectTransform { get; }
        int Id { get; }
        bool IsAlive { get; }
        string ActiveController { get; set; }

        T Get<T>(string id) where T : Object;
        T GetSetting<T>(string id);
    }
}