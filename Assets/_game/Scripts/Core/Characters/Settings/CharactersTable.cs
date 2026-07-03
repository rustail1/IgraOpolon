using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Characters/CharactersTable", fileName = "CharactersTable")]
    public class CharactersTable : ScriptableObject
    {
        [field: SerializeField]
        public CharacterSettings[] Characters { get; private set; }
    }
}
