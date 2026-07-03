using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public sealed class CharacterNavigation
    {
        private const float DestinationUpdateDistance = 0.1f;
        private const float NavMeshSampleDistance = 2f;
        private const float MinimumFacingSpeed = 0.1f;
        private const float MinimumTurnAngle = 4f;
        private const float TurnJitterWindow = 1f;
        private const float TurnLockDuration = 0.5f;
        private const int MaximumTurnDirectionChanges = 4;

        private readonly NavMeshAgent _agent;
        private Vector3 _destination;
        private float _stoppingDistance;
        private float _turnJitterWindowStartedTime;
        private float _turnLockedUntilTime;
        private int _lastTurnDirection;
        private int _turnDirectionChanges;
        private bool _hasDestination;
        private bool _isAvailable;

        public float Radius => _agent.radius;

        public bool HasDestination => _hasDestination;

        public Vector3 Destination => _destination;

        public bool IsMoving => _isAvailable && !_agent.isStopped &&
            _agent.velocity.sqrMagnitude > Mathf.Epsilon;

        public CharacterNavigation(CharacterView characterView, float movementSpeed, int avoidancePriority)
        {
            _agent = characterView.NavigationAgent;
            _agent.speed = movementSpeed;
            _agent.autoBraking = false;
            _agent.updateRotation = false;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
            _agent.avoidancePriority = avoidancePriority;
            _isAvailable = TryGetNavMeshPosition(_agent.transform.position, out var navigationPosition);
            if (!_isAvailable)
            {
                Debug.LogError($"{nameof(CharacterNavigation)}: {characterView.name} is not on a NavMesh for agent type {_agent.agentTypeID}.");

                return;
            }

            _agent.Warp(navigationPosition);
        }

        public bool MoveTo(Vector3 destination, float stoppingDistance)
        {
            if (!_isAvailable || !TryGetNavMeshPosition(destination, out var navigationDestination))
            {
                return false;
            }

            if (!NeedsDestinationUpdate(navigationDestination, stoppingDistance))
            {
                return true;
            }

            _destination = navigationDestination;
            _stoppingDistance = stoppingDistance;
            _agent.stoppingDistance = stoppingDistance;
            _agent.isStopped = false;
            _hasDestination = _agent.SetDestination(navigationDestination);
            _agent.isStopped = !_hasDestination;

            return _hasDestination;
        }

        public bool HasReachedDestination()
        {
            if (!_isAvailable || !_hasDestination || _agent.pathPending ||
                _agent.pathStatus != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            var hasReachedDestination = _agent.remainingDistance <= _stoppingDistance;

            return hasReachedDestination;
        }

        public void Stop()
        {
            if (!_isAvailable)
            {
                return;
            }

            _agent.isStopped = true;
            _agent.ResetPath();
            _hasDestination = false;
        }

        public void Disable()
        {
            Stop();
            _agent.enabled = false;
        }

        public void UpdateFacing()
        {
            if (!IsMoving)
            {
                return;
            }

            FaceDirection(_agent.velocity, false);
        }

        public void FacePosition(Vector3 position)
        {
            var direction = position - _agent.transform.position;
            direction.y = 0f;
            FaceDirection(direction, false);
        }

        public void ForceFacePosition(Vector3 position)
        {
            var direction = position - _agent.transform.position;
            direction.y = 0f;
            FaceDirection(direction, true);
        }

        private void FaceDirection(Vector3 direction, bool isForced)
        {
            direction.y = 0f;
            if (!_isAvailable || direction.sqrMagnitude <= MinimumFacingSpeed * MinimumFacingSpeed ||
                !isForced && (Time.time < _turnLockedUntilTime || IsTurnJittering(direction)))
            {
                return;
            }

            _agent.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        private bool IsTurnJittering(Vector3 direction)
        {
            if (Time.time >= _turnJitterWindowStartedTime + TurnJitterWindow)
            {
                _turnJitterWindowStartedTime = Time.time;
                _lastTurnDirection = 0;
                _turnDirectionChanges = 0;
            }

            var signedAngle = Vector3.SignedAngle(_agent.transform.forward, direction, Vector3.up);
            if (Mathf.Abs(signedAngle) < MinimumTurnAngle)
            {
                return false;
            }

            var turnDirection = signedAngle > 0f ? 1 : -1;
            if (_lastTurnDirection == 0 || _lastTurnDirection == turnDirection)
            {
                _lastTurnDirection = turnDirection;

                return false;
            }

            _lastTurnDirection = turnDirection;
            _turnDirectionChanges++;
            if (_turnDirectionChanges <= MaximumTurnDirectionChanges)
            {
                return false;
            }

            _turnLockedUntilTime = Time.time + TurnLockDuration;
            _turnJitterWindowStartedTime = Time.time;
            _turnDirectionChanges = 0;

            return true;
        }

        private bool NeedsDestinationUpdate(Vector3 destination, float stoppingDistance)
        {
            if (!_hasDestination || !Mathf.Approximately(_stoppingDistance, stoppingDistance))
            {
                return true;
            }

            var destinationOffset = destination - _destination;
            var needsUpdate = destinationOffset.sqrMagnitude >=
                DestinationUpdateDistance * DestinationUpdateDistance;

            return needsUpdate;
        }

        private bool TryGetNavMeshPosition(Vector3 position, out Vector3 navigationPosition)
        {
            var queryFilter = new NavMeshQueryFilter
            {
                agentTypeID = _agent.agentTypeID,
                areaMask = _agent.areaMask
            };
            var hasNavigationPosition = NavMesh.SamplePosition(
                position,
                out var navMeshHit,
                NavMeshSampleDistance,
                queryFilter);
            navigationPosition = navMeshHit.position;

            return hasNavigationPosition;
        }
    }
}
