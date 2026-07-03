using UnityEngine;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class BaseProductionSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private BaseSettings _baseSettings;
        private ICurrenciesModel _resourcesModel;
        private float _nextBaseProductionTime;

        [Inject]
        private void Construct(
            BaseSettings baseSettings,
            ICurrenciesModel resourcesModel)
        {
            _baseSettings = baseSettings;
            _resourcesModel = resourcesModel;
        }

        protected override void OnInitialize()
        {
            _nextBaseProductionTime = Time.time + _baseSettings.Production.Interval;
        }

        public void OnUpdate()
        {
            if (Time.time < _nextBaseProductionTime)
            {
                return;
            }

            _resourcesModel.Add(CurrencyType.Gold, _baseSettings.Production.Amount);
            _nextBaseProductionTime = Time.time + _baseSettings.Production.Interval;
        }
    }
}
