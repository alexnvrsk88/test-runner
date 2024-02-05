using System;
using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [Serializable]
    public sealed class PlayerParameterEffectConfig
    {
        [SerializeField] private PlayerParameter _parameter;
        [SerializeField] private float _value;
        [SerializeField] private bool _isMultiplier;

        public PlayerParameter Parameter => _parameter;
        public float Value => _value;
        public bool IsMultiplier => _isMultiplier;
    }
}