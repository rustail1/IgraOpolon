using R3;
using UnityEngine;

namespace Game
{
    public sealed class ConstructionModel : IConstructionModel
    {
        private readonly ReactiveProperty<int> _level = new(1);
        private readonly ReactiveProperty<float> _buildElapsedTime = new(0f);
        private readonly ReactiveProperty<bool> _isBuilt = new(false);

        public ConstructionType ConstructionType { get; }

        public ConstructionSettings Settings { get; }

        public GameObject View { get; }

        public ReadOnlyReactiveProperty<int> Level => _level;

        public ReadOnlyReactiveProperty<float> BuildElapsedTime => _buildElapsedTime;

        public ReadOnlyReactiveProperty<bool> IsBuilt => _isBuilt;

        [SerializeField] private readonly int needWorkers = 5;

        public ConstructionModel(
            ConstructionType constructionType,
            ConstructionSettings settings,
            GameObject view)
        {
            ConstructionType = constructionType;
            Settings = settings;
            View = view;
            View.SetActive(false);
        }

        public void AdvanceBuild(float deltaTime)
        {
            if (_isBuilt.Value)
            {
                return;
            }

            _buildElapsedTime.Value = Mathf.Min(
                _buildElapsedTime.Value + deltaTime,
                Settings.BuildDuration);

            if (_buildElapsedTime.Value < Settings.BuildDuration)
            {
                return;
            }
            _isBuilt.Value = true;
            View.SetActive(true);
        }
        public bool EnoughWorkers()
        {
            return CurrenciesModel.Instance.Followers.Value >= needWorkers;
        }
        public int NeedWorkers()
        {
            return needWorkers;
        }
        public void Upgrade()
        {
            if (!_isBuilt.Value)
            {
                return;
            }

            _level.Value++;
        }
    }
}
