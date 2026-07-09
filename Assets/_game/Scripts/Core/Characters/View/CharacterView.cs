using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class CharacterView : MonoBehaviour
    {
        private const string _outlineLayerName = "Outlined";
        private const string _defaultLayer = "Character";
        private const float _healthBarWidth = 42f;
        private const float _healthBarHeight = 6f;

        [field: SerializeField]
        public CapsuleCollider AlliesCollider { get; private set; }

        [field: SerializeField]
        public CapsuleCollider EnemiesCollider { get; private set; }

        [field: SerializeField]
        public NavMeshAgent NavigationAgent { get; private set; }

        [field: SerializeField]
        public Animator Animator { get; private set; }

        [field: SerializeField]
        public CharacterViewAnimationEvents AnimationEvents { get; private set; }

        [field: SerializeField]
        public SkinnedMeshRenderer[] SkinnedMeshRenderers { get; private set; }

        public Transform RootTransform => transform;

        public Transform SkeletonRoot => Animator.transform;

        public OutpostTeam Team => _characterModel.Team;

        public event Action AttackHit;

        public event Action AttackEnded;

        private Vector3 _defaultVisualLocalPosition;
        private Quaternion _defaultVisualLocalRotation;
        private Camera _healthBarCamera;
        
        private int _maximumHealth;
        private float _healthBarVerticalOffset;
        private Color _healthBarColor;

        private CharacterModel _characterModel;
        public int Health;
        private void Awake()
        {
            _defaultVisualLocalPosition = SkeletonRoot.localPosition;
            _defaultVisualLocalRotation = SkeletonRoot.localRotation;
            AnimationEvents.OnPunchHit += NotifyAttackHit;
            AnimationEvents.OnPunchEnded += NotifyAttackEnded;
        }

        private void OnDestroy()
        {
            AnimationEvents.OnPunchHit -= NotifyAttackHit;
            AnimationEvents.OnPunchEnded -= NotifyAttackEnded;
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || _characterModel == null || !_characterModel.Navigation.HasDestination)
            {
                return;
            }

            var destination = _characterModel.Navigation.Destination;
            Gizmos.color = Team == OutpostTeam.Player ? Color.cyan : Color.red;
            Gizmos.DrawLine(RootTransform.position, destination);
            Gizmos.DrawWireSphere(destination, _characterModel.Navigation.Radius);
        }

        private void OnGUI()
        {
            if (_healthBarCamera == null || Health <= 0)
            {
                return;
            }

            var worldPosition = RootTransform.position + Vector3.up * _healthBarVerticalOffset;
            var screenPosition = _healthBarCamera.WorldToScreenPoint(worldPosition);
            if (screenPosition.z <= 0f)
            {
                return;
            }

            var healthRatio = (float)Health / _maximumHealth;
            var positionX = screenPosition.x - _healthBarWidth * 0.5f;
            var positionY = Screen.height - screenPosition.y;
            var backgroundRect = new Rect(positionX, positionY, _healthBarWidth, _healthBarHeight);
            var healthRect = new Rect(positionX, positionY, _healthBarWidth * healthRatio, _healthBarHeight);
            GUI.color = Color.black;
            GUI.DrawTexture(backgroundRect, Texture2D.whiteTexture);
            GUI.color = _healthBarColor;
            GUI.DrawTexture(healthRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        public void SetPose(Vector3 position, Quaternion rotation)
        {
            transform.SetPositionAndRotation(position, rotation);
        }

        public void Initialize(CharacterModel characterModel)
        {
            _characterModel = characterModel;
            AddTeamMaterial();
        }

        public void SetHealthBar(
            Camera camera,
            int health,
            int maximumHealth,
            Color color,
            float verticalOffset)
        {
            _healthBarCamera = camera;
            Health = health;
            _maximumHealth = maximumHealth;
            _healthBarColor = color;
            _healthBarVerticalOffset = verticalOffset;
        }

        public void SetMoving(bool isMoving)
        {
            if (_characterModel == null)
            {
                return;
            }

            Animator.SetFloat(_characterModel.CharacterSettings.AnimationsSettings.SpeedAnimationParameter, isMoving ? 1f : 0f);
        }

        public void ResetAnimationSpeed()
        {
            Animator.speed = 1f;
        }

        public void PlayAttack()
        {
            if (_characterModel == null)
            {
                return;
            }

            var attackClipLength = _characterModel.CharacterSettings.AnimationsSettings.AttackClipLength;
            Animator.speed = attackClipLength * _characterModel.CharacterSettings.AttackSpeed;
            Animator.SetTrigger(_characterModel.CharacterSettings.AnimationsSettings.AttackAnimationTrigger);
        }

        public void PlayDying()
        {
            if (_characterModel == null)
            {
                return;
            }

            Animator.speed = 1f;
            Animator.SetTrigger(_characterModel.CharacterSettings.AnimationsSettings.DyingAnimationTrigger);
        }
        
        public void ResetVisualRoot()
        {
            SkeletonRoot.localPosition = _defaultVisualLocalPosition;
            SkeletonRoot.localRotation = _defaultVisualLocalRotation;
        }

        public void ResetVisualRootPosition()
        {
            SkeletonRoot.localPosition = _defaultVisualLocalPosition;
        }

        private void NotifyAttackHit()
        {
            AttackHit?.Invoke();
        }

        private void NotifyAttackEnded()
        {
            ResetAnimationSpeed();
            AttackEnded?.Invoke();
        }

        private void AddTeamMaterial()
        {
            var teamSettings = Team == OutpostTeam.Player
                ? _characterModel.Settings.TeamA
                : _characterModel.Settings.TeamB;
            foreach (var skinnedMeshRenderer in SkinnedMeshRenderers)
            {
                var materials = skinnedMeshRenderer.sharedMaterials;
                var teamMaterials = new Material[materials.Length + 1];
                Array.Copy(materials, teamMaterials, materials.Length);
                teamMaterials[^1] = teamSettings.Material;
                skinnedMeshRenderer.sharedMaterials = teamMaterials;
            }
        }
    }
}
