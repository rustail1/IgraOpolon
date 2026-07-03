#if UNITY_EDITOR
using UnityEditor;
#endif
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Characters/CharacterSettings", fileName = "CharacterSettings")]
    public class CharacterSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: BoxGroup("Info")]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Info")]
        [field: TextArea(2, 5)]
        public string Bio { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("View")]
        public CharacterView CharacterPrefab { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("View")]
        public Sprite Icon { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Animation")]
        public CharacterAnimationsSettings AnimationsSettings { get; private set; }
        
        [field: SerializeField]
        [field: BoxGroup("Cost")]
        public int GoldCost { get; private set; } = 50;

        [field: SerializeField]
        [field: BoxGroup("Cost")]
        public int FollowersCost { get; private set; } = 1;

        [field: SerializeField]
        [field: BoxGroup("Movement")]
        public float BaseMovementSpeed { get; private set; } = 4f;

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Stats")]
        public int Attack { get; private set; } = 13;

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Stats")]
        public int Defense { get; private set; }

        [field: SerializeField]
        [field: MinValue(0.01f)]
        [field: BoxGroup("Stats")]
        public float AttackSpeed { get; private set; } = 1f;

        [field: SerializeField]
        [field: MinValue(1)]
        [field: BoxGroup("Stats")]
        public int Health { get; private set; } = 420;
    }
}
