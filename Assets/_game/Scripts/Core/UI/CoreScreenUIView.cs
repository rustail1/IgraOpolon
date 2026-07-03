using UnityEngine;
using UnityEngine.UI;
using Whaledevelop.UI;

namespace Game
{
    public class CoreScreenUIView : UIView<CoreScreenUIViewModel>
    {
        [SerializeField]
        private ResourcesUIView _resourcesPrefab;

        [SerializeField]
        private Transform _resourcesRoot;

        [SerializeField]
        private Text _waveTimerText;

        private CoreScreenUIViewModel _model;
        private ResourcesUIView _resourcesView;

        public override void Initialize(CoreScreenUIViewModel model)
        {
            _model = model;
            _model.WaveTimerChanged += RefreshWaveTimer;
            _model.MatchChanged += RefreshWaveTimer;
            _resourcesView = Instantiate(_resourcesPrefab, _resourcesRoot);
            model.InitializeSubViews(_resourcesView);
        }

        public override void Release()
        {
            _model.WaveTimerChanged -= RefreshWaveTimer;
            _model.MatchChanged -= RefreshWaveTimer;
            _model.ReleaseSubViews(_resourcesView);
            Destroy(_resourcesView.gameObject);
            _resourcesView = null;
            _model = null;
        }

        private void RefreshWaveTimer()
        {
            _waveTimerText.text =
                $"End: {_model.TimeRemaining}\n" +
                $"Next wave: {_model.TimeUntilNextWave}\n" +
                $"Base: {_model.PlayerBaseHealth} | Altar: {_model.EnemyAltarHealth} | {_model.MatchState}";
        }
    }
}
