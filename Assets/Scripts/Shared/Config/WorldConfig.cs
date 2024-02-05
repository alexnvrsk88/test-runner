using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [Serializable]
    public sealed class WorldConfig
    {
        [SerializeField] private List<TokenConfig> _tokenConfigs;
        
        [Tooltip("Token spawn rate in seconds")]
        [SerializeField] private float _tokenSpawnRate;

        public IReadOnlyList<TokenConfig> TokenConfigs => _tokenConfigs;
        public float TokenSpawnRate => _tokenSpawnRate;
    }
}