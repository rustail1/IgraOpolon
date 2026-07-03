using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Teams/TeamSettings", fileName = "TeamSettings")]
    public class TeamSettings : ScriptableObject
    {
        [field: SerializeField]
        public bool IsActive { get; private set; } = true;

        [field: SerializeField]
        public Color Color { get; private set; } = Color.white;

        [field: SerializeField]
        public Material Material { get; private set; }
    }
}
