using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;

namespace Game
{
    public class FreeRoamCameraView : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [SerializeField]
        private FreeRoamCameraSettings _settings;

        [BoxGroup("References")]
        [SerializeField]
        private Transform _pivot;

        [BoxGroup("References")]
        [SerializeField]
        private CinemachineCamera _cinemachineCamera;

        [BoxGroup("References")]
        [SerializeField]
        private CinemachineFollow _cinemachineFollow;

        [BoxGroup("References")]
        [SerializeField]
        private CinemachineHardLookAt _cinemachineHardLookAt;

        private void Awake()
        {
            _cinemachineFollow.FollowOffset = _settings.FollowOffset;
            _cinemachineHardLookAt.LookAtOffset = _settings.LookOffset;
            _cinemachineCamera.Follow = _pivot;
            _cinemachineCamera.LookAt = _pivot;
        }

        public void UpdateCamera(
            Vector2 movementInput,
            float rotationInput,
            float zoomInput,
            float deltaTime)
        {
            var followOffset = _cinemachineFollow.FollowOffset;
            MovePivot(movementInput, followOffset, deltaTime);
            Rotate(rotationInput, ref followOffset, deltaTime);
            Zoom(zoomInput, ref followOffset);
            _cinemachineFollow.FollowOffset = followOffset;

            return;
        }

        public void ApplyStartPose(Transform source)
        {
            var lookDistance = _settings.FollowOffset.magnitude;
            var lookPosition = source.position + source.forward * lookDistance;
            var pivotPosition = lookPosition - source.rotation * _settings.LookOffset;
            var followOffset = source.position - pivotPosition;

            _pivot.SetPositionAndRotation(pivotPosition, source.rotation);
            _cinemachineFollow.FollowOffset = followOffset;
            _cinemachineCamera.PreviousStateIsValid = false;
        }

        private void MovePivot(Vector2 movementInput, Vector3 followOffset, float deltaTime)
        {
            if (movementInput == Vector2.zero)
            {
                return;
            }

            var forward = -Vector3.ProjectOnPlane(followOffset, Vector3.up).normalized;
            var right = Vector3.Cross(Vector3.up, forward);
            var movement = right * movementInput.x + forward * movementInput.y;
            _pivot.position += movement * _settings.MovementSpeed * deltaTime;

            return;
        }

        private void Rotate(float rotationInput, ref Vector3 followOffset, float deltaTime)
        {
            if (rotationInput == 0f)
            {
                return;
            }

            var rotation = Quaternion.AngleAxis(
                rotationInput * _settings.RotationSpeed * deltaTime,
                Vector3.up);
            followOffset = rotation * followOffset;

            return;
        }

        private void Zoom(float zoomInput, ref Vector3 followOffset)
        {
            if (zoomInput == 0f)
            {
                return;
            }

            var distance = Mathf.Clamp(
                followOffset.magnitude - zoomInput * _settings.ZoomSpeed,
                _settings.MinDistance,
                _settings.MaxDistance);
            followOffset = followOffset.normalized * distance;

            return;
        }
    }
}
