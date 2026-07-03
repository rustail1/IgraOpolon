using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Whaledevelop.UI;

namespace Game
{
    public sealed class PlayerSquadPanelUIView : UIView<PlayerSquadPanelUIViewModel>
    {
        [SerializeField]
        private Button[] _lineButtons;

        [SerializeField]
        private Image[] _lineHighlights;

        [SerializeField]
        private Text _lineTitleText;

        [SerializeField]
        private Transform _queuedCharactersRoot;

        [SerializeField]
        private Transform _availableCharactersRoot;

        [SerializeField]
        private PlayerSquadCardUIView _cardPrefab;

        private readonly List<PlayerSquadCardUIView> _queuedCharacterCards = new();
        private readonly List<PlayerSquadCardUIView> _availableCharacterCards = new();
        private PlayerSquadPanelUIViewModel _model;

        public override void Initialize(PlayerSquadPanelUIViewModel model)
        {
            _model = model;

            CreateCards(_queuedCharacterCards, _queuedCharactersRoot, 6);
            CreateCards(_availableCharacterCards, _availableCharactersRoot, _model.AvailableCharacters.Count);
            _model.SquadChanged += Refresh;

            for (var i = 0; i < _lineButtons.Length; i++)
            {
                var lineCode = (LineCode)i;
                _lineHighlights[i].raycastTarget = false;
                _lineButtons[i].onClick.AddListener(() => SelectLine(lineCode));
            }

            Refresh();
        }

        public override void Release()
        {
            for (var i = 0; i < _lineButtons.Length; i++)
            {
                _lineButtons[i].onClick.RemoveAllListeners();
            }

            ReleaseCards(_queuedCharacterCards);
            ReleaseCards(_availableCharacterCards);
            _model.SquadChanged -= Refresh;
            _model = null;
        }

        private void CreateCards(List<PlayerSquadCardUIView> cards, Transform root, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var card = Instantiate(_cardPrefab, root);
                cards.Add(card);
            }
        }

        private void ReleaseCards(List<PlayerSquadCardUIView> cards)
        {
            foreach (var card in cards)
            {
                card.Release();
                Destroy(card.gameObject);
            }

            cards.Clear();
        }

        private void SelectLine(LineCode lineCode)
        {
            _model.SelectLine(lineCode);
            Refresh();
        }

        private void Refresh()
        {
            _lineTitleText.text = $"{_model.SelectedLine.ToString().ToUpper()} WAVE";

            for (var i = 0; i < _lineHighlights.Length; i++)
            {
                _lineHighlights[i].enabled = _model.SelectedLine == (LineCode)i;
            }

            var queuedCharacters = _model.QueuedCharacters;

            for (var i = 0; i < _queuedCharacterCards.Count; i++)
            {
                if (i >= queuedCharacters.Count)
                {
                    _queuedCharacterCards[i].SetEmpty();

                    continue;
                }

                var characterIndex = i;
                var canRemove = !_model.IsSpawnLocked;
                _queuedCharacterCards[i].Initialize(
                    queuedCharacters[i],
                    canRemove ? () => TryRemoveQueuedCharacter(characterIndex) : null,
                    true,
                    canRemove ? "X" : "LOCK");
            }

            var availableCharacters = _model.AvailableCharacters;

            for (var i = 0; i < _availableCharacterCards.Count; i++)
            {
                var characterSettings = availableCharacters[i];
                _availableCharacterCards[i].Initialize(characterSettings, () => TryQueueCharacter(characterSettings), true);
            }
        }

        private void TryQueueCharacter(CharacterSettings characterSettings)
        {
            _model.TryQueueCharacter(characterSettings);
            Refresh();
        }

        private void TryRemoveQueuedCharacter(int characterIndex)
        {
            _model.TryRemoveQueuedCharacter(characterIndex);
            Refresh();
        }
    }
}
