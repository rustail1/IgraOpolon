using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public sealed class PlayerSquadCardUIView : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Text _nameText;

        [SerializeField]
        private Text _costText;

        private Action _selected;

        public void Initialize(
            CharacterSettings settings,
            Action selected,
            bool showCost,
            string actionText = null)
        {
            _button.onClick.RemoveListener(Select);
            _icon.raycastTarget = false;
            _nameText.raycastTarget = false;
            _costText.raycastTarget = false;
            _selected = selected;
            _icon.sprite = settings.Icon;
            _icon.color = Color.white;
            _nameText.text = settings.DisplayName;
            _costText.gameObject.SetActive(showCost);
            _costText.text = actionText ?? $"{settings.FollowersCost} | {settings.GoldCost}";
            _button.gameObject.SetActive(true);
            _button.onClick.AddListener(Select);
        }

        public void SetEmpty()
        {
            _button.gameObject.SetActive(false);
        }

        public void Release()
        {
            _button.onClick.RemoveListener(Select);
            _selected = null;
        }

        private void Select()
        {
            _selected?.Invoke();
        }
    }
}
