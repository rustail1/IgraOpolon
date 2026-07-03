using System;
using System.Collections.Generic;
using R3;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed partial class ConstructionsBehaviourSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private readonly List<IDisposable> _buildSubscriptions = new();
        private IConstructionsModel _constructionsModel;
        private ICurrenciesModel _resourcesModel;

        [Inject]
        private void Construct(
            IConstructionsModel constructionsModel,
            ICurrenciesModel resourcesModel)
        {
            _constructionsModel = constructionsModel;
            _resourcesModel = resourcesModel;
        }

        protected override void OnInitialize()
        {
            _constructionsModel.ConstructionAdded += OnConstructionAdded;

            foreach (var constructionModel in _constructionsModel.Constructions)
            {
                OnConstructionAdded(constructionModel);
            }
        }

        protected override void OnRelease()
        {
            _constructionsModel.ConstructionAdded -= OnConstructionAdded;

            foreach (var buildSubscription in _buildSubscriptions)
            {
                buildSubscription.Dispose();
            }

            _buildSubscriptions.Clear();
            _goldMineBehaviours.Clear(); 
        }

        private void OnConstructionAdded(IConstructionModel constructionModel)
        {
            var buildSubscription = constructionModel.IsBuilt.Subscribe(isBuilt =>
            {
                if (!isBuilt)
                {
                    return;
                }

                InitializeConstructionBehaviour(constructionModel);
            });
            _buildSubscriptions.Add(buildSubscription);
        }

        private void InitializeConstructionBehaviour(IConstructionModel constructionModel)
        {
            switch (constructionModel.Settings.BehaviourData)
            {
                case BarracksConstructionBehaviour behaviour:
                    InitializeBarracksBehaviour(constructionModel, behaviour);
                    break;
                case ForgeConstructionBehaviour behaviour:
                    InitializeForgeBehaviour(constructionModel, behaviour);
                    break;
                case FollowersHouseConstructionBehaviour behaviour:
                    InitializeFollowersHouseBehaviour(constructionModel, behaviour);
                    break;
                case GoldMineConstructionBehaviour behaviour:
                    InitializeGoldMineBehaviour(constructionModel, behaviour);
                    break;
            }
        }
    }
}
