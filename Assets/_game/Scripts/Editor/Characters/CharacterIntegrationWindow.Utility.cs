#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public sealed partial class CharacterIntegrationWindow
    {
        private string GetCharacterAssetName()
        {
            var assetName = _skeletonPrefab.name.Replace(" ", string.Empty);
            if (assetName.EndsWith(SkeletonPrefabSuffix))
            {
                assetName = assetName[..^SkeletonPrefabSuffix.Length];
            }

            return assetName;
        }

        private string GetSkeletonPrefabAssetName()
        {
            var assetName = $"{GetCharacterAssetName()}{SkeletonPrefabSuffix}";

            return assetName;
        }

        private string GetCharacterPrefabAssetName()
        {
            var assetName = $"{CharacterPrefix}{GetCharacterName()}{CharacterPrefabSuffix}";

            return assetName;
        }

        private string GetCharacterSettingsAssetName()
        {
            var assetName = $"{CharacterPrefix}{GetCharacterName()}{CharacterSettingsSuffix}";

            return assetName;
        }

        private string GetCharacterName()
        {
            var assetName = GetCharacterAssetName();
            if (assetName.StartsWith(CharacterPrefix))
            {
                assetName = assetName[CharacterPrefix.Length..];
            }

            return assetName;
        }

        private string GetSkeletonPrefabPath()
        {
            var sourcePath = AssetDatabase.GetAssetPath(_skeletonPrefab);
            var folder = Path.GetDirectoryName(sourcePath)?.Replace("\\", "/");
            var path = $"{folder}/{GetSkeletonPrefabAssetName()}.prefab";

            return path;
        }

        private string GetCharacterPrefabPath()
        {
            var path = $"{_characterPrefabsFolder}/{GetCharacterPrefabAssetName()}.prefab";

            return path;
        }

        private string GetCharacterSettingsPath()
        {
            var path = $"{_characterSettingsFolder}/{GetCharacterSettingsAssetName()}.asset";

            return path;
        }

        private static T FindComponentInHierarchy<T>(Transform root) where T : Component
        {
            var component = FindComponentOnObject<T>(root.gameObject);
            if (component != null)
            {
                return component;
            }

            foreach (Transform child in root)
            {
                component = FindComponentInHierarchy<T>(child);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private static void FindComponentsInHierarchy<T>(Transform root, List<T> components) where T : Component
        {
            var component = FindComponentOnObject<T>(root.gameObject);
            if (component != null)
            {
                components.Add(component);
            }

            foreach (Transform child in root)
            {
                FindComponentsInHierarchy(child, components);
            }

            return;
        }

        private static T FindComponentOnObject<T>(GameObject gameObject) where T : Component
        {
            var serializedObject = new SerializedObject(gameObject);
            var componentsProperty = serializedObject.FindProperty("m_Component");
            for (var i = 0; i < componentsProperty.arraySize; i++)
            {
                var componentProperty = componentsProperty.GetArrayElementAtIndex(i).FindPropertyRelative("component");
                if (componentProperty.objectReferenceValue is T component)
                {
                    return component;
                }
            }

            return null;
        }
    }
}
#endif
