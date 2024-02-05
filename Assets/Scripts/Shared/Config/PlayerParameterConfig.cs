using System;
using UnityEngine;

namespace Runner.Shared.Shared.Config
{
    [Serializable]
    public class PlayerParameterConfig
    {
        [SerializeField] private PlayerParameter _parameter;
        [SerializeField] private float _value;

        public PlayerParameter Parameter => _parameter;
        public float Value => _value;
    }
}