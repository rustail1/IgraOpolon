using UnityEngine;

namespace Game
{
    public sealed class CharacterGroup
    {
        private readonly float _slotSpacing;
        private readonly Vector3 _spawnPosition;
        private readonly Vector3 _spawnDirection;
        private Vector3 _currentCheckpoint;
        private Vector3 _direction;

        public CharacterGroup(float slotSpacing, Vector3 spawnPosition, Vector3 spawnForward)
        {
            _slotSpacing = slotSpacing;
            _spawnPosition = spawnPosition;
            _spawnDirection = GetHorizontalDirection(spawnForward, Vector3.forward);
            _currentCheckpoint = spawnPosition;
            _direction = _spawnDirection;
        }

        public Vector3 GetSpawnPosition(int characterIndex)
        {
            var spawnPosition = _spawnPosition + GetSlotOffset(characterIndex, _spawnDirection);

            return spawnPosition;
        }

        public Vector3 GetDestinationPosition(int characterIndex, Vector3 destination)
        {
            UpdateDestination(destination);
            var targetPosition = _currentCheckpoint + GetSlotOffset(characterIndex, _direction);

            return targetPosition;
        }

        private void UpdateDestination(Vector3 destination)
        {
            var offset = destination - _currentCheckpoint;
            offset.y = 0f;
            if (offset.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            _direction = offset.normalized;
            _currentCheckpoint = destination;
        }

        private Vector3 GetSlotOffset(int characterIndex, Vector3 direction)
        {
            if (characterIndex == 0)
            {
                return Vector3.zero;
            }

            var offset = -direction * (characterIndex * _slotSpacing);

            return offset;
        }

        private static Vector3 GetHorizontalDirection(Vector3 direction, Vector3 fallback)
        {
            direction.y = 0f;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                return fallback;
            }

            var horizontalDirection = direction.normalized;

            return horizontalDirection;
        }
    }
}
