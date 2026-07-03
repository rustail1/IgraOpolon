using System;
using System.Collections.Generic;
using R3;
using VContainer;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    public sealed class PlayerSquadPanelUIViewModel : SyncLifetime, IUIViewModel
    {
        private ICharacterWaveModel _characterWaveModel;
        private IPlayerSquadsModel _playerSquadsModel;
        private CoreStateSettings _stateSettings;
        private IDisposable _squadSubscription;
        private IDisposable _spawnLockSubscription;

        public event Action SquadChanged;

        public bool IsSpawnLocked => _characterWaveModel.IsSpawnLocked.CurrentValue;

        public LineCode SelectedLine { get; private set; } = LineCode.Mid;

        public IReadOnlyList<CharacterSettings> AvailableCharacters => _stateSettings.StartSettings.AvailableCharacters.Characters;

        public IReadOnlyList<CharacterSettings> QueuedCharacters => _playerSquadsModel.GetQueuedCharacters(SelectedLine);

        [Inject]
        public void Construct(
            ICharacterWaveModel characterWaveModel,
            IPlayerSquadsModel playerSquadsModel,
            CoreStateSettings stateSettings)
        {
            _characterWaveModel = characterWaveModel;
            _playerSquadsModel = playerSquadsModel;
            _stateSettings = stateSettings;
        }

        protected override void OnInitialize()
        {
            _squadSubscription = _playerSquadsModel.Revision.Subscribe(_ => SquadChanged?.Invoke());
            _spawnLockSubscription = _characterWaveModel.IsSpawnLocked.Subscribe(_ => SquadChanged?.Invoke());
        }

        protected override void OnRelease()
        {
            _squadSubscription.Dispose();
            _spawnLockSubscription.Dispose();
        }

        public void SelectLine(LineCode lineCode)
        {
            SelectedLine = lineCode;
            SquadChanged?.Invoke();
        }

        public bool TryQueueCharacter(CharacterSettings characterSettings)
        {
            var isQueued = _playerSquadsModel.TryQueueCharacter(SelectedLine, characterSettings);

            return isQueued;
        }

        public bool TryRemoveQueuedCharacter(int characterIndex)
        {
            var isRemoved = _playerSquadsModel.TryRemoveQueuedCharacter(SelectedLine, characterIndex);

            return isRemoved;
        }
    }
}
