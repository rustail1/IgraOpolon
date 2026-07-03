#if UNITY_EDITOR
using UnityEditor;

namespace Game
{
    public sealed partial class CharacterIntegrationWindow
    {
        private void ClearPreviousIntegration()
        {
            var settings = AssetDatabase.LoadAssetAtPath<CharacterSettings>(GetCharacterSettingsPath());
            RemoveFromCharactersTable(settings);
            DeleteAssetIfExists(GetCharacterSettingsPath());
            DeleteAssetIfExists(GetCharacterPrefabPath());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return;
        }

        private void RemoveFromCharactersTable(CharacterSettings settings)
        {
            var serializedTable = new SerializedObject(_charactersTable);
            var charactersProperty = serializedTable.FindProperty(CharactersBackingField);
            var settingsName = GetCharacterSettingsAssetName();

            for (var i = charactersProperty.arraySize - 1; i >= 0; i--)
            {
                var characterProperty = charactersProperty.GetArrayElementAtIndex(i);
                var characterSettings = characterProperty.objectReferenceValue as CharacterSettings;
                if ((settings != null && characterSettings == settings) ||
                    (characterSettings != null && characterSettings.name == settingsName))
                {
                    DeleteArrayElement(charactersProperty, i);
                }
            }

            serializedTable.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(_charactersTable);

            return;
        }

        private static void DeleteAssetIfExists(string assetPath)
        {
            if (AssetDatabase.LoadMainAssetAtPath(assetPath) == null)
            {
                return;
            }

            AssetDatabase.DeleteAsset(assetPath);

            return;
        }

        private static void DeleteArrayElement(SerializedProperty arrayProperty, int index)
        {
            var arraySize = arrayProperty.arraySize;
            arrayProperty.DeleteArrayElementAtIndex(index);
            if (arrayProperty.arraySize == arraySize)
            {
                arrayProperty.DeleteArrayElementAtIndex(index);
            }

            return;
        }
    }
}
#endif
