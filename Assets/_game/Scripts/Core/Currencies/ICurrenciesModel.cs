using R3;

namespace Game
{
    public interface ICurrenciesModel
    {
        ReactiveProperty<int> Gold { get; }

        ReactiveProperty<int> Followers { get; }

        void Add(CurrencyType currencyType, int amount);

        bool TrySpend(CurrencyType currencyType, int amount);
    }
}
