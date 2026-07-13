using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class ConstructionSlotUIView : MonoBehaviour
    {
        private const float PositionStabilityThreshold = 0.1f;
        private const int RequiredStablePositionFrames = 2;
        private const string BuildingText = "Building...";
        private const int BuildingTextFontSize = 24;

        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private GameObject _previewRoot;

        [SerializeField]
        private Button _selectionButton;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private GameObject _detailsPanel;

        [SerializeField]
        private Button _detailsBackgroundButton;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Text _descriptionText;

        [SerializeField]
        private Text _levelText;

        [SerializeField]
        private Text _priceText;

        [SerializeField]
        private Button _actionButton;

        [SerializeField]
        private Text _actionButtonText;

        private ConstructionSlotUIViewModel _model;
        private Vector3 _previousScreenPosition;
        private Vector2 _nameDefaultAnchorMin;
        private Vector2 _nameDefaultAnchorMax;
        private Vector2 _nameDefaultAnchoredPosition;
        private Vector2 _nameDefaultSizeDelta;
        private int _nameDefaultFontSize;
        private int _stablePositionFrames;
        private bool _hasScreenPosition;
        private bool _isInitialPositionReady;

        public void Initialize(ConstructionSlotUIViewModel model)
        {
            gameObject.SetActive(false);
            _model = model;
            _nameDefaultAnchorMin = _nameText.rectTransform.anchorMin;
            _nameDefaultAnchorMax = _nameText.rectTransform.anchorMax;
            _nameDefaultAnchoredPosition = _nameText.rectTransform.anchoredPosition;
            _nameDefaultSizeDelta = _nameText.rectTransform.sizeDelta;
            _nameDefaultFontSize = _nameText.fontSize;
            _icon.sprite = _model.Settings.Icon;
            _nameText.text = _model.Settings.DisplayName;
            _descriptionText.text = _model.Settings.Description;
            _selectionButton.onClick.AddListener(ToggleDetails);
            _detailsBackgroundButton.onClick.AddListener(CloseDetails);
            _closeButton.onClick.AddListener(CloseDetails);
            _actionButton.onClick.AddListener(_model.ExecuteAction);
            _model.Changed += Refresh;
            SetDetailsActive(false);
            Refresh();
        }

        public void UpdateScreenPosition()
        {
            var screenPosition = _model.Camera.WorldToScreenPoint(
                _model.Slot.position + Vector3.up * (_model.IsBuilt ? 1f : 3f));
            _rectTransform.position = screenPosition;

            if (!_isInitialPositionReady)
            {
                UpdateInitialPositionState(screenPosition);
                gameObject.SetActive(_isInitialPositionReady && screenPosition.z > 0f);

                return;
            }

            gameObject.SetActive(screenPosition.z > 0f);
        }

        public void Release()
        {
            _model.Changed -= Refresh;
            _selectionButton.onClick.RemoveListener(ToggleDetails);
            _detailsBackgroundButton.onClick.RemoveListener(CloseDetails);
            _closeButton.onClick.RemoveListener(CloseDetails);
            _actionButton.onClick.RemoveListener(_model.ExecuteAction);
            _model = null;
        }

        private void ToggleDetails()
        {
            SetDetailsActive(!_detailsPanel.activeSelf);
        }

        private void CloseDetails()
        {
            SetDetailsActive(false);
        }

        private void SetDetailsActive(bool isActive)
        {
            _detailsBackgroundButton.gameObject.SetActive(isActive);
            _detailsPanel.SetActive(isActive);
        }

        private void UpdateInitialPositionState(Vector3 screenPosition)
        {
            if (!_hasScreenPosition)
            {
                _previousScreenPosition = screenPosition;
                _hasScreenPosition = true;

                return;
            }

            var positionDelta = screenPosition - _previousScreenPosition;
            _stablePositionFrames = positionDelta.sqrMagnitude <= PositionStabilityThreshold * PositionStabilityThreshold
                ? _stablePositionFrames + 1
                : 0;
            _previousScreenPosition = screenPosition;
            _isInitialPositionReady = _stablePositionFrames >= RequiredStablePositionFrames;
        }

        private void Refresh()
        {
            var isBuilding = _model.IsBuilding;
            _previewRoot.SetActive(!_model.IsBuilt);
            _icon.gameObject.SetActive(!isBuilding);
            RefreshName(isBuilding);
            _levelText.text = GetLevelText();
            _priceText.text = $"{_model.ActionPrice} GOLD";
            _priceText.gameObject.SetActive(!isBuilding);
            _actionButtonText.text = GetActionButtonText();
            _actionButton.interactable = !isBuilding && _model.Gold >= _model.ActionPrice;
        }

        private void RefreshName(bool isBuilding)
        {
            var nameRectTransform = _nameText.rectTransform;
            _nameText.text = isBuilding ? BuildingText : _model.Settings.DisplayName;
            _nameText.fontSize = isBuilding ? BuildingTextFontSize : _nameDefaultFontSize;
            nameRectTransform.anchorMin = isBuilding ? new Vector2(0f, 0) : _nameDefaultAnchorMin;
            nameRectTransform.anchorMax = isBuilding ? new Vector2(1f, 1f) : _nameDefaultAnchorMax;
            nameRectTransform.anchoredPosition = isBuilding ? Vector2.zero : _nameDefaultAnchoredPosition;
            nameRectTransform.sizeDelta = isBuilding ? new Vector2(150f, 24f) : _nameDefaultSizeDelta;
        }

        private string GetLevelText()
        {
            if (_model.IsBuilding)
            {
                return "BUILDING";
            }

            return _model.IsBuilt ? $"LEVEL {_model.Level}" : "NOT BUILT";
        }

        private string GetActionButtonText()
        {
            if (!_model.IsBuilding)
            {
                return _model.IsBuilt ? "UPGRADE" : "BUILD";
            }

            return $"{FormatTime(_model.BuildElapsedTime)}/{FormatTime(_model.Settings.BuildDuration)}";
        }

        private static string FormatTime(float time)
        {
            var totalSeconds = Mathf.FloorToInt(time);
            var timeSpan = TimeSpan.FromSeconds(totalSeconds);

            return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:00}";
        }
    }
}
