using R3;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class BaseModel : GameModel<BaseModel, IBaseModel>, IBaseModel
    {
        private BaseSettings _settings;

        public ReactiveProperty<int> Health { get; } = new();

        public int MaxHealth => _settings.MaxHealth;

        [Inject]
        private void Construct(BaseSettings settings)
        {
            _settings = settings;
        }

        protected override void OnInitialize()
        {
            Health.Value = _settings.MaxHealth;
        }

        public void ApplyDamage(int damage)
        {
            Health.Value = System.Math.Max(Health.Value - damage, 0);
        }
    }
}