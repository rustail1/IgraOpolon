using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "CoreStateSettings", menuName = "Game/Core/CoreStateSettings")]
    public class CoreStateSettings : ScriptableObject
    {
        [field: SerializeField]
        [field: BoxGroup("Camera")]
        public FreeRoamCameraView FreeRoamCameraPrefab { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Level")]
        public LevelSettings LevelSettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("UI")]
        public UGUISettings UGUISettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("UI")]
        public BattleUISettings BattleUISettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Characters")]
        public CharactersTable CharactersTable { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Base")]
        public BaseSettings BaseSettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Outposts")]
        public OutpostsSettings OutpostsSettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Start")]
        public StartSettings StartSettings { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Construction")]
        public ConstructionSettings[] ConstructionSettings { get; private set; }

        public ConstructionSettings GetConstructionSettings(ConstructionType constructionType)
        {
            foreach (var constructionSettings in ConstructionSettings)
            {
                if (constructionSettings.ConstructionType == constructionType)
                {
                    return constructionSettings;
                }
            }

            return null;
        }
    }
}
