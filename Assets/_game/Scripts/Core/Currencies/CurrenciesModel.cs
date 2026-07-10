using R3;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class CurrenciesModel
        : GameModel<CurrenciesModel, ICurrenciesModel>, ICurrenciesModel
    {
        /// <summary>
        /// Позволяет получать модель из обычных MonoBehaviour
        /// (например TowerBuildMenu), которые не создаются VContainer.
        /// </summary>
        public static CurrenciesModel Instance { get; private set; }

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
            Instance = this;

            Gold.Value = _stateSettings.StartSettings.StartingGold;
            Followers.Value = _stateSettings.StartSettings.StartingFollowers;
        }

        protected override void OnRelease()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void Add(CurrencyType currencyType, int amount)
        {
            switch (currencyType)
            {
                case CurrencyType.Gold:
                    Gold.Value += amount;
                    break;

                default:
                    Followers.Value += amount;
                    break;
            }
        }

        public bool TrySpend(CurrencyType currencyType, int amount)
        {
            ReactiveProperty<int> resource =
                currencyType == CurrencyType.Gold
                    ? Gold
                    : Followers;

            if (resource.Value < amount)
            {
                return false;
            }

            resource.Value -= amount;
            return true;
        }
    }
}