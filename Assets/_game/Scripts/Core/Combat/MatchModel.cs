using R3;
using VContainer;
using UnityEngine;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class MatchModel : GameModel<MatchModel, IMatchModel>, IMatchModel
    {
        private StartSettings _settings;
        private float _remainingTime;
        private float _stateElapsedTime;
        private bool _isTimerExpired;
        private readonly ReactiveProperty<int> _secondsRemaining = new();
        private readonly ReactiveProperty<int> _playerBaseHealth = new();
        private readonly ReactiveProperty<int> _enemyAltarHealth = new();
        private readonly ReactiveProperty<MatchState> _state = new(MatchState.Playing);
        

        public ReadOnlyReactiveProperty<int> SecondsRemaining => _secondsRemaining;

        public ReadOnlyReactiveProperty<int> PlayerBaseHealth => _playerBaseHealth;

        public ReadOnlyReactiveProperty<int> EnemyAltarHealth => _enemyAltarHealth;

        public ReadOnlyReactiveProperty<MatchState> State => _state;

        public bool IsPlaying => _state.CurrentValue == MatchState.Playing;

        [Inject]
        private void Construct(CoreStateSettings stateSettings)
        {
            _settings = stateSettings.StartSettings;
        }

        protected override void OnInitialize()
        {
            _remainingTime = _settings.MatchDurationSeconds;
            _stateElapsedTime = 0f;
            _isTimerExpired = false;
            _secondsRemaining.Value = _settings.MatchDurationSeconds;
            _playerBaseHealth.Value = _settings.PlayerBaseHealth;
            _enemyAltarHealth.Value = _settings.EnemyAltarHealth;
            _state.Value = MatchState.Playing;
        }

        public void Tick(float deltaTime)
        {
            if (_state.CurrentValue == MatchState.Playing)
            {
                if (_isTimerExpired)
                {
                    _state.Value = MatchState.Defeat;
                }
                else
                {
                    _remainingTime = System.Math.Max(_remainingTime - deltaTime, 0f);
                    _secondsRemaining.Value = Mathf.CeilToInt(_remainingTime);

                    if (_remainingTime <= 0f)
                    {
                        _isTimerExpired = true;
                        _state.Value = MatchState.Defeat;
                    }
                }
            }

            if (_state.CurrentValue == MatchState.PortalOpening ||
                _state.CurrentValue == MatchState.VictoryPending)
            {
                _stateElapsedTime += deltaTime;

                float duration = _state.CurrentValue == MatchState.PortalOpening
                    ? _settings.PortalOpeningDuration
                    : _settings.VictoryPendingDuration;

                if (_stateElapsedTime >= duration)
                {
                    _stateElapsedTime = 0f;

                    _state.Value = _state.CurrentValue == MatchState.PortalOpening
                        ? MatchState.VictoryPending
                        : MatchState.Victory;
                }
            }

            if (_state.CurrentValue == MatchState.Victory)
            {
                GameResultWindow.Instance.gameObject.SetActive(true);
                GameResultWindow.Instance.SetText("You win!");
            }
            else if (_state.CurrentValue == MatchState.Defeat)
            {
                GameResultWindow.Instance.gameObject.SetActive(true);
                GameResultWindow.Instance.SetText("You lose!");
            }
        }
        public void ApplyDamage(OutpostTeam targetTeam, int damage)
        {
            if (!IsPlaying || damage <= 0)
            {
                return;
            }

            if (targetTeam == OutpostTeam.Player)
            {
                _playerBaseHealth.Value = System.Math.Max(_playerBaseHealth.CurrentValue - damage, 0);
                if (_playerBaseHealth.CurrentValue == 0)
                {
                    _state.Value = MatchState.Defeat;
                }

                return;
            }

            _enemyAltarHealth.Value = System.Math.Max(_enemyAltarHealth.CurrentValue - damage, 0);
            if (_enemyAltarHealth.CurrentValue > 0)
            {
                return;
            }

            _state.Value = MatchState.PortalOpening;
            _stateElapsedTime = 0f;
        }
    }
}
