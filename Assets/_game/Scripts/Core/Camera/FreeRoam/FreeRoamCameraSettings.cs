using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "FreeRoamCameraSettings", menuName = "Game/Camera/FreeRoamCameraSettings")]
    public class FreeRoamCameraSettings : ScriptableObject
    {
        [BoxGroup("Position")]
        [SerializeField]
        private Vector3 _followOffset = new(8f, 12f, -8f);

        [BoxGroup("Position")]
        [SerializeField]
        private Vector3 _lookOffset = new(0f, 1f, 0f);

        [BoxGroup("Movement")]
        [SerializeField]
        private float _movementSpeed = 12f;

        [BoxGroup("Rotation")]
        [SerializeField]
        private float _rotationSpeed = 90f;

        [BoxGroup("Zoom")]
        [SerializeField]
        private float _zoomSpeed = 0.5f;

        [BoxGroup("Zoom")]
        [SerializeField]
        private float _minDistance = 8f;

        [BoxGroup("Zoom")]
        [SerializeField]
        private float _maxDistance = 24f;

        public Vector3 FollowOffset => _followOffset;

        public Vector3 LookOffset => _lookOffset;

        public float MovementSpeed => _movementSpeed;

        public float RotationSpeed => _rotationSpeed;

        public float ZoomSpeed => _zoomSpeed;

        public float MinDistance => _minDistance;

        public float MaxDistance => _maxDistance;
    }
}
