using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "BattleUISettings", menuName = "Game/UI/BattleUISettings")]
    public sealed class BattleUISettings : ScriptableObject
    {
        [field: SerializeField]
        [field: MinValue(0f)]
        [field: BoxGroup("Health Bars")]
        public float HealthBarVerticalOffset { get; private set; } = 2.6f;
    }
}
