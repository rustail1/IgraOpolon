#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public sealed partial class CharacterIntegrationWindow : OdinEditorWindow
    {
        private const string SkeletonPrefabSuffix = "_Skeleton";
        private const string CharacterPrefix = "Character_";
        private const string CharacterPrefabSuffix = "_View";
        private const string CharacterSettingsSuffix = "_Settings";
        private const string CharacterPrefabBackingField = "<CharacterPrefab>k__BackingField";
        private const string CharactersBackingField = "<Characters>k__BackingField";
        private const string DisplayNameBackingField = "<DisplayName>k__BackingField";
        private const string AnimatorBackingField = "<Animator>k__BackingField";
        private const string SkinnedMeshRenderersBackingField = "<SkinnedMeshRenderers>k__BackingField";
        private const string AvatarProperty = "m_Avatar";
        private const string ControllerProperty = "m_Controller";

        [BoxGroup("Input")]
        [Required]
        [SerializeField]
        private GameObject _skeletonPrefab;

        [BoxGroup("Input")]
        [Required]
        [SerializeField]
        private CharactersTable _charactersTable;

        [BoxGroup("Input")]
        [Required]
        [SerializeField]
        private CharacterView _characterViewBase;

        [BoxGroup("Animation")]
        [Required]
        [SerializeField]
        private RuntimeAnimatorController _animationController;

        [BoxGroup("Animation")]
        [Required]
        [SerializeField]
        private Avatar _avatar;

        [BoxGroup("Paths")]
        [FolderPath]
        [SerializeField]
        private string _characterPrefabsFolder = "Assets/_game/Content/Characters/Prefabs";

        [BoxGroup("Paths")]
        [SerializeField]
        [FolderPath]
        private string _characterSettingsFolder = "Assets/_game/Content/Characters/Settings";

        [MenuItem("Tools/Game/Character Integration")]
        private static void Open()
        {
            var window = GetWindow<CharacterIntegrationWindow>();
            window.titleContent = new GUIContent("Character Integration");
            window.Show();

            return;
        }

        [Button("Integrate Character", ButtonSizes.Large)]
        private void IntegrateCharacter()
        {
            if (!ValidateInput())
            {
                return;
            }

            ClearPreviousIntegration();

            if (!PrepareSkeletonPrefab())
            {
                return;
            }

            var prefab = CreateCharacterPrefab();
            var settings = CreateCharacterSettings(prefab);
            AddToCharactersTable(settings);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = settings;
            Debug.Log($"{nameof(CharacterIntegrationWindow)}: Integrated {settings.name}.");

            return;
        }

        private bool ValidateInput()
        {
            if (_skeletonPrefab == null ||
                _charactersTable == null ||
                _characterViewBase == null ||
                _animationController == null ||
                _avatar == null)
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Skeleton prefab, characters table, character view base, animation controller and avatar must be specified.");

                return false;
            }

            var sourcePath = AssetDatabase.GetAssetPath(_skeletonPrefab);
            if (string.IsNullOrEmpty(sourcePath) || !sourcePath.EndsWith(".prefab"))
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Skeleton prefab must be a prefab project asset.");

                return false;
            }

            var skeletonPath = GetSkeletonPrefabPath();
            if (sourcePath != skeletonPath && AssetDatabase.LoadMainAssetAtPath(skeletonPath) != null)
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Skeleton prefab target path already exists: {skeletonPath}.");

                return false;
            }

            if (!PrefabUtility.IsPartOfPrefabAsset(_characterViewBase))
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Character view base must be a prefab asset.");

                return false;
            }

            if (FindComponentOnObject<Animator>(_skeletonPrefab) == null)
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Skeleton prefab root must contain Animator.");

                return false;
            }

            if (FindComponentInHierarchy<SkinnedMeshRenderer>(_skeletonPrefab.transform) == null)
            {
                Debug.LogError($"{nameof(CharacterIntegrationWindow)}: Skeleton prefab must contain SkinnedMeshRenderer.");

                return false;
            }

            return true;
        }
    }
}
#endif
