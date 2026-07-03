#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public sealed class AssetsIntegrationWindow : OdinEditorWindow
    {
        [SerializeField]
        private GameObject _root;

        [SerializeField]
        [BoxGroup("Remove materials")]
        private Material _targetMaterial;

        [MenuItem("Tools/Game/AssetsIntegrationWindow")]
        private static void Open()
        {
            GetWindow<AssetsIntegrationWindow>("AssetsIntegrationWindow");
        }

        [Button(ButtonSizes.Large)]
        [BoxGroup("Remove materials")]
        private void RemoveMaterialsExceptTarget()
        {
            if (_root == null || _targetMaterial == null)
            {
                Debug.LogError("Root or Target Material is not assigned.");
                return;
            }

            var undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Remove Materials Except Target");

            var renderers = _root.GetComponentsInChildren<Renderer>(true);
            var changedCount = 0;

            foreach (var renderer in renderers)
            {
                var oldMaterials = renderer.sharedMaterials;
                var newMaterials = new List<Material>();

                foreach (var material in oldMaterials)
                {
                    if (material == _targetMaterial)
                    {
                        newMaterials.Add(material);
                    }
                }

                if (newMaterials.Count == oldMaterials.Length)
                {
                    continue;
                }

                Undo.RecordObject(renderer, "Remove Materials Except Target");

                renderer.sharedMaterials = newMaterials.ToArray();

                EditorUtility.SetDirty(renderer);
                changedCount++;
            }

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"Done. Checked renderers: {renderers.Length}. Changed renderers: {changedCount}");
        }

        [SerializeField]
        [BoxGroup("Inside box remover")]
        private BoxCollider _interiorBoxCollider;

        [Button(ButtonSizes.Large)]
        [BoxGroup("Inside box remover")]
        private void RemoveObjectsInsideBoxCollider()
        {
            if (_root == null)
            {
                Debug.LogError("Root is not assigned.");
                return;
            }

            if (_interiorBoxCollider == null)
            {
                Debug.LogError("Interior BoxCollider is not assigned.");
                return;
            }

            var undoGroup = Undo.GetCurrentGroup();
            Undo.SetCurrentGroupName("Remove Objects Inside BoxCollider");

            var renderers = _root.GetComponentsInChildren<MeshRenderer>(true);
            var destroyedObjects = new HashSet<GameObject>();
            var removedCount = 0;

            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                if (renderer.transform == _root.transform)
                {
                    continue;
                }

                if (renderer.transform == _interiorBoxCollider.transform)
                {
                    continue;
                }


                if (!IsRendererInsideBoxCollider(renderer, _interiorBoxCollider))
                {
                    Debug.Log("Not inside");
                    continue;
                }

                var targetObject = renderer.gameObject;

                if (!destroyedObjects.Add(targetObject))
                {
                    continue;
                }

                Undo.DestroyObjectImmediate(targetObject);
                removedCount++;
            }

            Undo.CollapseUndoOperations(undoGroup);

            Debug.Log($"Done. Checked renderers: {renderers.Length}. Removed objects: {removedCount}");
        }
        
        private bool IsRendererInsideBoxCollider(Renderer renderer, BoxCollider boxCollider)
        {
            var bounds = renderer.bounds;

            var points = new[]
            {
                bounds.center,
                bounds.min,
                bounds.max,
                new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
                new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
                new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            };

            foreach (var point in points)
            {
                var closestPoint = boxCollider.ClosestPoint(point);

                if ((closestPoint - point).sqrMagnitude < 0.0001f)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif