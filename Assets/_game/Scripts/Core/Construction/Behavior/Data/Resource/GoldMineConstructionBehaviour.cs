using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [Serializable]
    public sealed class GoldMineConstructionBehaviour : IConstructionBehaviourData
    {
        [field: SerializeField]
        [field: BoxGroup("Resources")]
        public ResourceProductionSettings Production { get; private set; } = new();

        public float NextProductionTime { get; set; }
    }
}
