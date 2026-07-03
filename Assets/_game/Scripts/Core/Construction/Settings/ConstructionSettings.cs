using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ConstructionSettings", menuName = "Game/Construction/Settings")]
    public sealed class ConstructionSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: BoxGroup("Identity")]
        public ConstructionType ConstructionType { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Identity")]
        public string DisplayName { get; private set; }

        [field: SerializeField]
        [field: TextArea]
        [field: BoxGroup("Presentation")]
        public string Description { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Presentation")]
        public GameObject Prefab { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Presentation")]
        public Sprite Icon { get; private set; }

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Economy")]
        public int BuildPrice { get; private set; }

        [field: SerializeField]
        [field: MinValue(0.1f)]
        [field: BoxGroup("Construction")]
        public float BuildDuration { get; private set; }

        [field: SerializeField]
        [field: MinValue(0)]
        [field: BoxGroup("Economy")]
        public int UpgradePrice { get; private set; }

        [field: SerializeField]
        [field: SerializeReference]
        [field: BoxGroup("Behaviour")]
        public IConstructionBehaviourData BehaviourData { get; private set; }
    }
}
