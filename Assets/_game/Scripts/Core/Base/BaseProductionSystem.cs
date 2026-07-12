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
        private float _nextGoldProductionTime;
        private float _nextFollowerProductionTime;
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
            _nextGoldProductionTime = Time.time + _baseSettings.Production.Interval;
            _nextFollowerProductionTime = Time.time + _baseSettings.Production.Interval;  
        }

        public void OnUpdate()
        {
            if (Time.time < _nextFollowerProductionTime)
            {
                return;
            }
            _resourcesModel.Add(CurrencyType.Followers, _baseSettings.Production.Amount);

            float followersProductionIncrease = 1f;
            if (FollowersHouse.Instance != null) followersProductionIncrease *= 2f;
            _nextFollowerProductionTime = Time.time + _baseSettings.Production.Interval / followersProductionIncrease;

            if (Time.time < _nextGoldProductionTime || GoldMine.Instance != null)
            {
                return;
            }
            _resourcesModel.Add(CurrencyType.Gold, _baseSettings.Production.Amount);
            _nextGoldProductionTime = Time.time + _baseSettings.Production.Interval;
        }
    }
}
