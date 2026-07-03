using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "StartSettings", menuName = "Game/Core/StartSettings")]
    public sealed class StartSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("StartResources")]
        public int StartingGold { get; private set; }

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("StartResources")]
        public int StartingFollowers { get; private set; } = 10;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Waves")]
        public float FirstWaveCooldown { get; private set; } = 10f;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Waves")]
        public float WaveCooldown { get; private set; } = 30f;

        [field: SerializeField]
        [field: BoxGroup("Waves")]
        public CharactersTable AvailableCharacters { get; private set; }

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Waves")]
        public int MaximumPlayerCharactersPerLine { get; private set; } = 5;

        [field: SerializeField]
        [field: BoxGroup("Teams")]
        public TeamSettings TeamA { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Teams")]
        public TeamSettings TeamB { get; private set; }

        [field: SerializeField]
        [field: MinValue(0.1f)]
        [field: BoxGroup("Combat")]
        public float UnitAttackRange { get; private set; } = 1.5f;

        [field: SerializeField]
        [field: MinValue(0.1f)]
        [field: BoxGroup("Combat")]
        public float UnitAggroRange { get; private set; } = 8f;

        [field: SerializeField]
        [field: MinValue(0f)]
        [field: BoxGroup("Waves")]
        public float SpawnLockDuration { get; private set; } = 1.5f;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Match")]
        public int MatchDurationSeconds { get; private set; } = 720;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Match")]
        public int PlayerBaseHealth { get; private set; } = 2000;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Match")]
        public int EnemyAltarHealth { get; private set; } = 2000;

        [field: SerializeField]
        [field: MinValue(0f)]
        [field: BoxGroup("Match")]
        public float PortalOpeningDuration { get; private set; } = 1f;

        [field: SerializeField]
        [field: MinValue(0f)]
        [field: BoxGroup("Match")]
        public float VictoryPendingDuration { get; private set; } = 2f;
    }
}
