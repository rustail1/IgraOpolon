using System.Collections.Generic;
using R3;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class PlayerSquadsModel : GameModel<PlayerSquadsModel, IPlayerSquadsModel>, IPlayerSquadsModel
    {
        private readonly Dictionary<LineCode, List<CharacterSettings>> _queuedCharacters = new();
        private readonly ReactiveProperty<int> _revision = new();
        private StartSettings _startSettings;
        private ICurrenciesModel _currenciesModel;
        private ICharacterWaveModel _characterWaveModel;

        public ReadOnlyReactiveProperty<int> Revision => _revision;

        [Inject]
        private void Construct(
            CoreStateSettings stateSettings,
            ICurrenciesModel currenciesModel,
            ICharacterWaveModel characterWaveModel)
        {
            _startSettings = stateSettings.StartSettings;
            _currenciesModel = currenciesModel;
            _characterWaveModel = characterWaveModel;
        }

        protected override void OnInitialize()
        {
            foreach (LineCode lineCode in System.Enum.GetValues(typeof(LineCode)))
            {
                _queuedCharacters.Add(lineCode, new List<CharacterSettings>());
            }
        }

        public IReadOnlyList<CharacterSettings> GetQueuedCharacters(LineCode lineCode) => _queuedCharacters[lineCode];

        public bool TryQueueCharacter(LineCode lineCode, CharacterSettings characterSettings)
        {
            var queuedCharacters = _queuedCharacters[lineCode];
            if (_characterWaveModel.IsSpawnLocked.CurrentValue ||
                queuedCharacters.Count >= _startSettings.MaximumPlayerCharactersPerLine ||
                _currenciesModel.Gold.Value < characterSettings.GoldCost)
            {

                return false;
            }

            _currenciesModel.TrySpend(CurrencyType.Gold, characterSettings.GoldCost);
            queuedCharacters.Add(characterSettings);
            _revision.Value++;

            return true;
        }

        public bool TryRemoveQueuedCharacter(LineCode lineCode, int characterIndex)
        {
            var queuedCharacters = _queuedCharacters[lineCode];
            if (_characterWaveModel.IsSpawnLocked.CurrentValue || characterIndex < 0 || characterIndex >= queuedCharacters.Count)
            {
                return false;
            }

            var characterSettings = queuedCharacters[characterIndex];
            queuedCharacters.RemoveAt(characterIndex);
            _currenciesModel.Add(CurrencyType.Gold, characterSettings.GoldCost);
            _revision.Value++;

            return true;
        }

        public IReadOnlyList<CharacterSettings> TakeQueuedCharacters(LineCode lineCode)
        {
            var queuedCharacters = _queuedCharacters[lineCode];
            var charactersForWave = queuedCharacters.ToArray();
            queuedCharacters.Clear();
            _revision.Value++;

            return charactersForWave;
        }
    }
}
