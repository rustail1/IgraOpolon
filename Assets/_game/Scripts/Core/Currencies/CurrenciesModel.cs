using R3;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class CurrenciesModel : GameModel<CurrenciesModel, ICurrenciesModel>, ICurrenciesModel
    {
        private CoreStateSettings _stateSettings;

        public ReactiveProperty<int> Gold { get; } = new();

        public ReactiveProperty<int> Followers { get; } = new();

        [Inject]
        private void Construct(CoreStateSettings stateSettings)
        {
            _stateSettings = stateSettings;
        }

        protected override void OnInitialize()
        {
            Gold.Value = _stateSettings.StartSettings.StartingGold;
            Followers.Value = _stateSettings.StartSettings.StartingFollowers;
        }

        public void Add(CurrencyType currencyType, int amount)
        {
            if (currencyType == CurrencyType.Gold)
            {
                Gold.Value += amount;

                return;
            }

            Followers.Value += amount;
        }

        public bool TrySpend(CurrencyType currencyType, int amount)
        {
            var resource = currencyType == CurrencyType.Gold ? Gold : Followers;

            if (resource.Value < amount)
            {
                return false;
            }

            resource.Value -= amount;

            return true;
        }
    }
}
