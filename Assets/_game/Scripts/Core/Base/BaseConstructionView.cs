using UnityEngine;
using Whaledevelop.Utility;

namespace Game
{
    public sealed class BaseConstructionView : MonoBehaviour
    {
        private const float _healthBarWidth = 120f;
        private const float _healthBarHeight = 9f;

        [SerializeField]
        private SerializableDictionary<ConstructionType, Transform> _constructionSlots;

        private Camera _healthBarCamera;
        private int _health;
        private int _maximumHealth;
        private float _healthBarVerticalOffset;
        private Color _healthBarColor;

        private void OnGUI()
        {
            if (_healthBarCamera == null || _health <= 0)
            {
                return;
            }

            var worldPosition = transform.position + Vector3.up * _healthBarVerticalOffset;
            var screenPosition = _healthBarCamera.WorldToScreenPoint(worldPosition);
            if (screenPosition.z <= 0f)
            {
                return;
            }

            var healthRatio = (float)_health / _maximumHealth;
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

        public Transform GetConstructionSlot(ConstructionType constructionType) =>
            _constructionSlots[constructionType];

        public void SetHealthBar(
            Camera camera,
            int health,
            int maximumHealth,
            Color color,
            float verticalOffset)
        {
            _healthBarCamera = camera;
            _health = health;
            _maximumHealth = maximumHealth;
            _healthBarColor = color;
            _healthBarVerticalOffset = verticalOffset;
            gameObject.SetActive(health > 0);
        }
    }
}
