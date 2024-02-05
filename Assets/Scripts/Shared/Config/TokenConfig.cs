using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [Serializable]
    public sealed class TokenConfig
    {
        [SerializeField] private string _prefabName;
        
        [Tooltip("Weight to select when token appears")]
        [SerializeField] private int _spawnWeight = 100;
        
        [Tooltip("Time of effect")]
        [SerializeField] private float _effectTime;
        
        [Tooltip("Effects for the player when token are collected")]
        [SerializeField] private List<PlayerParameterEffectConfig> _parameterEffects;

        public string PrefabName => _prefabName;
        public int SpawnWeight => _spawnWeight;
        public float EffectTime => _effectTime;
        public IReadOnlyList<PlayerParameterEffectConfig> ParameterEffects => _parameterEffects;
    }
}