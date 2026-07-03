using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BaseSettings", menuName = "Game/Base/Settings")]
    public sealed class BaseSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Stats")]
        public int MaxHealth { get; private set; } = 100;

        [field: SerializeField]
        [field: BoxGroup("Resources")]
        public ResourceProductionSettings Production { get; private set; } = new();
    }
}
