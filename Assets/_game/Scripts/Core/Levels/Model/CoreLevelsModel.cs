using R3;
using UnityEngine;
using VContainer;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public class CoreLevelsModel : GameModel<CoreLevelsModel, ICoreLevelsModel>, ICoreLevelsModel
    {
        private readonly CoreStateSettings _stateSettings;
        private readonly ReactiveProperty<ILevelModel> _level = new();

        [Inject]
        public CoreLevelsModel(CoreStateSettings stateSettings)
        {
            _stateSettings = stateSettings;
        }

        public ReadOnlyReactiveProperty<ILevelModel> Level => _level;

        protected override void OnInitialize()
        {
            InitializeLevel(_stateSettings.LevelSettings);
        }

        protected override void OnRelease()
        {
            ReleaseLevel();
        }

        public ILevelModel InitializeLevel(LevelSettings levelSettings)
        {
            ReleaseLevel();

            var levelView = Object.Instantiate(levelSettings.LevelViewPrefab);
            var levelModel = new LevelModel(levelView, levelSettings);
            _level.Value = levelModel;

            return levelModel;
        }

        private void ReleaseLevel()
        {
            var levelModel = _level.Value;
            _level.Value = null;

            if (levelModel != null && levelModel.View)
            {
                Object.Destroy(levelModel.View.gameObject);
            }
        }
    }
}
