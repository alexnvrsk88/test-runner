using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [Serializable]
    public sealed class PlayerConfig
    {
        [SerializeField] private string _playerPrefabName;
        [SerializeField] private List<PlayerParameterConfig> _baseParameters;

        public string PlayerPrefabName => _playerPrefabName;

        public float GetBaseParameterValue(PlayerParameter playerParameter)
        {
            return _baseParameters.Find(p => p.Parameter == playerParameter).Value;
        }
    }
}