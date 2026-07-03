using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class ResourceProductionSettings
    {
        [field: SerializeField]
        [field: MinValue(1)]
        public int Amount { get; private set; } = 1;

        [field: SerializeField]
        [field: MinValue(0.01f)]
        public float Interval { get; private set; } = 1f;

        [field: SerializeField]
        public CurrencyType CurrencyType { get; private set; }
    }
}