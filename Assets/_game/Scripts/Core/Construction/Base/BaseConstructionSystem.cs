using UnityEngine;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;
using Object = UnityEngine.Object;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class BaseConstructionSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private IConstructionsModel _constructionsModel;
        private ICoreLevelsModel _levelsModel;
        private CoreStateSettings _stateSettings;

        [Inject]
        private void Construct(
            IConstructionsModel constructionsModel,
            ICoreLevelsModel levelsModel,
            CoreStateSettings stateSettings)
        {
            _constructionsModel = constructionsModel;
            _levelsModel = levelsModel;
            _stateSettings = stateSettings;
        }

        protected override void OnInitialize()
        {
            _constructionsModel.ConstructionBuildRequested += BuildConstruction;
        }

        protected override void OnRelease()
        {
            _constructionsModel.ConstructionBuildRequested -= BuildConstruction;
        }

        public void OnUpdate()
        {
            foreach (var constructionModel in _constructionsModel.Constructions)
            {
                constructionModel.AdvanceBuild(Time.deltaTime);
            }
        }

        private void BuildConstruction(ConstructionType constructionType)
        {
            if (_constructionsModel.TryGetConstruction(constructionType, out _))
            {
                return;
            }

            var constructionSettings = _stateSettings.GetConstructionSettings(constructionType);
            var baseConstructionView = _levelsModel.Level.CurrentValue.View.BaseConstructionView;
            var constructionSlot = baseConstructionView.GetConstructionSlot(
                constructionType);
            var constructionView = Object.Instantiate(
                constructionSettings.Prefab,
                constructionSlot);
            _constructionsModel.AddConstruction(new ConstructionModel(
                constructionType,
                constructionSettings,
                constructionView));
        }
    }
}
