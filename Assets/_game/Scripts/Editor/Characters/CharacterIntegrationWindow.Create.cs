#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public sealed partial class CharacterIntegrationWindow
    {
        private bool PrepareSkeletonPrefab()
        {
            var sourcePath = AssetDatabase.GetAssetPath(_skeletonPrefab);
            var skeletonPath = GetSkeletonPrefabPath();
            if (sourcePath != skeletonPath)
            {
                var renameError = AssetDatabase.RenameAsset(sourcePath, GetSkeletonPrefabAssetName());
                if (!string.IsNullOrEmpty(renameError))
                {
                    Debug.LogError($"{nameof(CharacterIntegrationWindow)}: {renameError}");

                    return false;
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                _skeletonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(skeletonPath);
            }

            var skeletonRoot = PrefabUtility.LoadPrefabContents(skeletonPath);
            var animator = FindComponentOnObject<Animator>(skeletonRoot);

            ApplyAnimatorReferences(animator);
            EditorUtility.SetDirty(animator);
            PrefabUtility.SaveAsPrefabAsset(skeletonRoot, skeletonPath);
            PrefabUtility.UnloadPrefabContents(skeletonRoot);
            _skeletonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(skeletonPath);

            return true;
        }

        private CharacterView CreateCharacterPrefab()
        {
            var instanceRoot = PrefabUtility.InstantiatePrefab(_characterViewBase.gameObject) as GameObject;
            var instance = FindComponentOnObject<CharacterView>(instanceRoot);
            var modelInstance = PrefabUtility.InstantiatePrefab(_skeletonPrefab, instance.transform) as GameObject;

            modelInstance.name = GetSkeletonPrefabAssetName();
            modelInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            instance.name = GetCharacterPrefabAssetName();

            ApplyCharacterViewReferences(instance, modelInstance);

            var prefabPath = GetCharacterPrefabPath();
            PrefabUtility.SaveAsPrefabAsset(instance.gameObject, prefabPath);
            DestroyImmediate(instance.gameObject);

            return AssetDatabase.LoadAssetAtPath<CharacterView>(prefabPath);
        }

        private void ApplyCharacterViewReferences(CharacterView characterView, GameObject modelRoot)
        {
            var skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
            var animator = FindComponentOnObject<Animator>(modelRoot);
            FindComponentsInHierarchy(modelRoot.transform, skinnedMeshRenderers);

            var serializedCharacterView = new SerializedObject(characterView);
            serializedCharacterView.FindProperty(AnimatorBackingField).objectReferenceValue = animator;

            var skinnedMeshRenderersProperty = serializedCharacterView.FindProperty(SkinnedMeshRenderersBackingField);
            skinnedMeshRenderersProperty.arraySize = skinnedMeshRenderers.Count;
            for (var i = 0; i < skinnedMeshRenderers.Count; i++)
            {
                skinnedMeshRenderersProperty.GetArrayElementAtIndex(i).objectReferenceValue = skinnedMeshRenderers[i];
            }

            serializedCharacterView.ApplyModifiedPropertiesWithoutUndo();

            return;
        }

        private CharacterSettings CreateCharacterSettings(CharacterView prefab)
        {
            var settings = CreateInstance<CharacterSettings>();
            settings.name = GetCharacterSettingsAssetName();

            var serializedSettings = new SerializedObject(settings);
            serializedSettings.FindProperty(DisplayNameBackingField).stringValue = ObjectNames.NicifyVariableName(GetCharacterAssetName());
            serializedSettings.FindProperty(CharacterPrefabBackingField).objectReferenceValue = prefab;
            serializedSettings.ApplyModifiedPropertiesWithoutUndo();

            AssetDatabase.CreateAsset(settings, GetCharacterSettingsPath());

            return settings;
        }

        private void AddToCharactersTable(CharacterSettings settings)
        {
            var serializedTable = new SerializedObject(_charactersTable);
            var charactersProperty = serializedTable.FindProperty(CharactersBackingField);
            for (var i = 0; i < charactersProperty.arraySize; i++)
            {
                if (charactersProperty.GetArrayElementAtIndex(i).objectReferenceValue == settings)
                {
                    return;
                }
            }

            charactersProperty.arraySize++;
            charactersProperty.GetArrayElementAtIndex(charactersProperty.arraySize - 1).objectReferenceValue = settings;
            serializedTable.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(_charactersTable);

            return;
        }

        private void ApplyAnimatorReferences(Animator animator)
        {
            var serializedAnimator = new SerializedObject(animator);
            serializedAnimator.FindProperty(AvatarProperty).objectReferenceValue = _avatar;
            serializedAnimator.FindProperty(ControllerProperty).objectReferenceValue = _animationController;
            serializedAnimator.ApplyModifiedPropertiesWithoutUndo();

            return;
        }
    }
}
#endif
