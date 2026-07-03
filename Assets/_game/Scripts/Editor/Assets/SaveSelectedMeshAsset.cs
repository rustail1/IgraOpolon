using UnityEditor;
using UnityEngine;

namespace Game
{
    public static class SaveSelectedMeshAsset
    {
        [MenuItem("GameObject/Game/Save Selected Mesh As Asset", false, 99999)]
        private static void SaveSelectedMesh()
        {
            var selected = Selection.activeGameObject;

            if (selected == null)
            {
                Debug.LogError("No GameObject selected.");
                return;
            }

            var meshFilter = selected.GetComponent<MeshFilter>();

            if (meshFilter == null || meshFilter.sharedMesh == null)
            {
                Debug.LogError("Selected object has no MeshFilter with mesh.");
                return;
            }

            var sourceMesh = meshFilter.sharedMesh;

            var path = EditorUtility.SaveFilePanelInProject(
                "Save Mesh Asset",
                sourceMesh.name + ".asset",
                "asset",
                "Choose where to save the mesh asset");

            if (string.IsNullOrEmpty(path))
                return;

            var meshCopy = Object.Instantiate(sourceMesh);
            meshCopy.name = sourceMesh.name;

            AssetDatabase.CreateAsset(meshCopy, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Undo.RecordObject(meshFilter, "Assign Saved Mesh Asset");
            meshFilter.sharedMesh = meshCopy;

            EditorUtility.SetDirty(meshFilter);

            Debug.Log($"Mesh saved: {path}");
        }
    }
}