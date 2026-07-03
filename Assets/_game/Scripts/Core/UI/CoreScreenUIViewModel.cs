using System;
using R3;
using VContainer;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    public class CoreScreenUIViewModel : SyncLifetime, IUIViewModel
    {
        private IObjectResolver _objectResolver;
        private ICharacterWaveModel _characterWaveModel;
        private IMatchModel _matchModel;
        private IDisposable _waveTimerSubscription;
        private IDisposable _matchSubscription;
        private IDisposable _matchTimeSubscription;
        private IDisposable _playerBaseHealthSubscription;
        private IDisposable _enemyAltarHealthSubscription;

        public event Action WaveTimerChanged;

        public event Action MatchChanged;

        public int SecondsUntilNextWave => _characterWaveModel.SecondsUntilNextWave.CurrentValue;

        public string TimeUntilNextWave => FormatTime(SecondsUntilNextWave);

        public int SecondsRemaining => _matchModel.SecondsRemaining.CurrentValue;

        public string TimeRemaining => FormatTime(SecondsRemaining);

        public int PlayerBaseHealth => _matchModel.PlayerBaseHealth.CurrentValue;

        public int EnemyAltarHealth => _matchModel.EnemyAltarHealth.CurrentValue;

        public MatchState MatchState => _matchModel.State.CurrentValue;

        [Inject]
        public void Construct(
            IObjectResolver objectResolver,
            ICharacterWaveModel characterWaveModel,
            IMatchModel matchModel)
        {
            _objectResolver = objectResolver;
            _characterWaveModel = characterWaveModel;
            _matchModel = matchModel;
        }

        public void InitializeSubViews(
            ResourcesUIView resourcesView)
        {
            _waveTimerSubscription = _characterWaveModel.SecondsUntilNextWave.Subscribe(
                _ => WaveTimerChanged?.Invoke());
            _matchSubscription = _matchModel.State.Subscribe(_ => MatchChanged?.Invoke());
            _matchTimeSubscription = _matchModel.SecondsRemaining.Subscribe(_ => MatchChanged?.Invoke());
            _playerBaseHealthSubscription = _matchModel.PlayerBaseHealth.Subscribe(_ => MatchChanged?.Invoke());
            _enemyAltarHealthSubscription = _matchModel.EnemyAltarHealth.Subscribe(_ => MatchChanged?.Invoke());

            var resourcesModel = new ResourcesUIViewModel();
            _objectResolver.Inject(resourcesModel);
            resourcesView.Initialize(resourcesModel);
            ((IInitializable)resourcesModel).Initialize();
        }

        public void ReleaseSubViews(params UIView[] views)
        {
            _waveTimerSubscription.Dispose();
            _matchSubscription.Dispose();
            _matchTimeSubscription.Dispose();
            _playerBaseHealthSubscription.Dispose();
            _enemyAltarHealthSubscription.Dispose();

            foreach (var view in views)
            {
                view.Release();
            }
        }

        private static string FormatTime(int seconds)
        {
            var minutes = seconds / 60;
            var remainingSeconds = seconds % 60;

            return $"{minutes:00}:{remainingSeconds:00}";
        }
    }
}
