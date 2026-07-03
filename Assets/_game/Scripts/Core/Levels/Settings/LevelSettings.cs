using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Level/LevelSettings", fileName = "LevelSettings")]
    public class LevelSettings : ScriptableObject
    {
        [field: SerializeField]
        public LevelView LevelViewPrefab { get; private set; }

    }
}
