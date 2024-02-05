using System.Linq;
using UnityEngine.LowLevel;

namespace Runner.Core.Utils
{
    public enum PlayerLoopLayer
    {
        TimeUpdate = 0,
        Initialization = 1,
        EarlyUpdate = 2,
        FixedUpdate = 3,
        PreUpdate = 4,
        Update = 5,
        PreLateUpdate = 6,
        PostLateUpdate = 7
    }

    public static class PlayerLoopUtility
    {
        public static void AddToPlayerLoop<TLoopType>(PlayerLoopLayer loopLayer, PlayerLoopSystem.UpdateFunction updateFunction) where TLoopType : struct
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var updateLoopSystem = currentPlayerLoop.subSystemList[(int)loopLayer];
            var updateLoopSystemList = updateLoopSystem.subSystemList.ToList();
            var componentsUpdateSystem = new PlayerLoopSystem
            {
                type = typeof(TLoopType),
                updateDelegate = updateFunction
            };
            updateLoopSystemList.Add(componentsUpdateSystem);
            updateLoopSystem.subSystemList = updateLoopSystemList.ToArray();
            currentPlayerLoop.subSystemList[(int)loopLayer] = updateLoopSystem;
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        public static void ResetPlayerLoop()
        {
            PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
        }
    }
}