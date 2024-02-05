using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [CreateAssetMenu(fileName = "GameConfig.asset", menuName = "Game/Create GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [SerializeField] private WorldConfig _worldConfig;
        [SerializeField] private PlayerConfig _playerConfig;
        
        public PlayerConfig PlayerConfig => _playerConfig;
        public WorldConfig WorldConfig => _worldConfig;
    }
}