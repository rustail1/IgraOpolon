using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "OutpostsSettings", menuName = "Game/Outposts/Settings")]
    public sealed class OutpostsSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: MinValue(0.1f)]
        public float BaseCaptureDuration { get; private set; } = 10f;

        [field: SerializeField]
        [field: MinValue(0.1f)]
        public float WaitingDistance { get; private set; } = 3f;
    }
}
