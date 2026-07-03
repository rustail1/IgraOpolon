using System;
using R3;
using UnityEngine;

namespace Game
{
    public sealed class ConstructionSlotUIViewModel
    {
        private readonly IConstructionsModel _constructionsModel;
        private readonly ICurrenciesModel _resourcesModel;
        private IDisposable _goldSubscription;
        private IDisposable _buildElapsedTimeSubscription;
        private IDisposable _isBuiltSubscription;
        private IDisposable _levelSubscription;
        private IConstructionModel _constructionModel;

        public event Action Changed;

        public ConstructionSettings Settings { get; }

        public Transform Slot { get; }

        public Camera Camera { get; }

        public bool IsBuilding => _constructionModel != null && !IsBuilt;

        public bool IsBuilt => _constructionModel?.IsBuilt.CurrentValue ?? false;

        public int Level => _constructionModel?.Level.CurrentValue ?? 0;

        public int ActionPrice => IsBuilt ? Settings.UpgradePrice : Settings.BuildPrice;

        public int Gold => _resourcesModel.Gold.CurrentValue;

        public float BuildElapsedTime => _constructionModel?.BuildElapsedTime.CurrentValue ?? 0f;

        public ConstructionSlotUIViewModel(
            ConstructionSettings settings,
            Transform slot,
            Camera camera,
            IConstructionsModel constructionsModel,
            ICurrenciesModel resourcesModel)
        {
            Settings = settings;
            Slot = slot;
            Camera = camera;
            _constructionsModel = constructionsModel;
            _resourcesModel = resourcesModel;
        }

        public void Initialize()
        {
            _constructionsModel.ConstructionAdded += OnConstructionAdded;
            _goldSubscription = _resourcesModel.Gold.Subscribe(_ => Changed?.Invoke());

            if (_constructionsModel.TryGetConstruction(Settings.ConstructionType, out var construction))
            {
                SetConstruction(construction);
            }
        }

        public void ExecuteAction()
        {
            if (IsBuilding)
            {
                return;
            }

            if (!_resourcesModel.TrySpend(CurrencyType.Gold, ActionPrice))
            {
                return;
            }

            if (_constructionModel == null)
            {
                _constructionsModel.RequestConstructionBuild(Settings.ConstructionType);

                return;
            }

            _constructionModel.Upgrade();
        }

        public void Release()
        {
            _constructionsModel.ConstructionAdded -= OnConstructionAdded;
            _goldSubscription.Dispose();
            _buildElapsedTimeSubscription?.Dispose();
            _isBuiltSubscription?.Dispose();
            _levelSubscription?.Dispose();
        }

        private void OnConstructionAdded(IConstructionModel constructionModel)
        {
            if (constructionModel.ConstructionType != Settings.ConstructionType)
            {
                return;
            }

            SetConstruction(constructionModel);
        }

        private void SetConstruction(IConstructionModel constructionModel)
        {
            _constructionModel = constructionModel;
            _buildElapsedTimeSubscription?.Dispose();
            _isBuiltSubscription?.Dispose();
            _levelSubscription?.Dispose();
            _buildElapsedTimeSubscription = _constructionModel.BuildElapsedTime.Subscribe(_ => Changed?.Invoke());
            _isBuiltSubscription = _constructionModel.IsBuilt.Subscribe(_ => Changed?.Invoke());
            _levelSubscription = _constructionModel.Level.Subscribe(_ => Changed?.Invoke());
        }
    }
}
