using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class CharacterSpawnSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private readonly List<PendingCharacterSpawn> _pendingSpawns = new();
        private CoreStateSettings _stateSettings;
        private ICharacterWaveModel _characterWaveModel;
        private IPlayerSquadsModel _playerSquadsModel;
        private ICoreLevelsModel _levelsModel;
        private IMatchModel _matchModel;
        private ICharacterCombatSystem _characterCombatSystem;
        private bool _isFirstWave = true;
        private float _remainingWaveTime;

        [Inject]
        private void Construct(
            CoreStateSettings stateSettings,
            ICharacterWaveModel characterWaveModel,
            IPlayerSquadsModel playerSquadsModel,
            ICoreLevelsModel levelsModel,
            IMatchModel matchModel,
            ICharacterCombatSystem characterCombatSystem)
        {
            _stateSettings = stateSettings;
            _characterWaveModel = characterWaveModel;
            _playerSquadsModel = playerSquadsModel;
            _levelsModel = levelsModel;
            _matchModel = matchModel;
            _characterCombatSystem = characterCombatSystem;
        }

        protected override void OnInitialize()
        {
            RestartWaveTimer();
        }

        public void OnUpdate()
        {
            _matchModel.Tick(Time.deltaTime);
            if (!_matchModel.IsPlaying || _matchModel.SecondsRemaining.CurrentValue == 0)
            {
                _characterWaveModel.SetSpawnLocked(true);

                return;
            }

            ProcessPendingSpawns();
            _remainingWaveTime -= Time.deltaTime;
            _characterWaveModel.SetSecondsUntilNextWave(Mathf.CeilToInt(Mathf.Max(_remainingWaveTime, 0f)));
            _characterWaveModel.SetSpawnLocked(
                _remainingWaveTime <= _stateSettings.StartSettings.SpawnLockDuration);

            if (_remainingWaveTime > 0f)
            {
                return;
            }

            SpawnWave();
            RestartWaveTimer();
        }

        private void SpawnWave()
        {
            var levelView = _levelsModel.Level.CurrentValue.View;
            if (!_stateSettings.StartSettings.TeamA.IsActive && !_stateSettings.StartSettings.TeamB.IsActive)
            {
                return;
            }

            SpawnMirroredWave(levelView);
        }

        private void SpawnMirroredWave(LevelView levelView)
        {
            foreach (LineCode lineCode in System.Enum.GetValues(typeof(LineCode)))
            {
                var characterSettings = _playerSquadsModel.TakeQueuedCharacters(lineCode);
                if (characterSettings.Count == 0)
                {
                    continue;
                }

                AddPendingSpawn(levelView, lineCode, OutpostTeam.Player, characterSettings);
                AddPendingSpawn(levelView, lineCode, OutpostTeam.Enemy, characterSettings);
            }
        }

        private void ProcessPendingSpawns()
        {
            if (_pendingSpawns.Count == 0)
            {
                return;
            }

            foreach (var pendingSpawn in _pendingSpawns)
            {
                SpawnCharacterGroup(
                    pendingSpawn.LevelView,
                    pendingSpawn.LineCode,
                    pendingSpawn.Team,
                    pendingSpawn.CharacterSettings);
            }

            _pendingSpawns.Clear();
        }

        private void AddPendingSpawn(
            LevelView levelView,
            LineCode lineCode,
            OutpostTeam team,
            IReadOnlyList<CharacterSettings> characterSettings)
        {
            if (team == OutpostTeam.Player && !_stateSettings.StartSettings.TeamA.IsActive ||
                team == OutpostTeam.Enemy && !_stateSettings.StartSettings.TeamB.IsActive)
            {
                return;
            }

            _pendingSpawns.Add(new PendingCharacterSpawn(levelView, lineCode, team, characterSettings));
        }

        private void SpawnCharacterGroup(
            LevelView levelView,
            LineCode lineCode,
            OutpostTeam team,
            IReadOnlyList<CharacterSettings> characterSettings)
        {
            var lineView = levelView.GetLine(lineCode);
            var spawnPoint = team == OutpostTeam.Player ? lineView.SpawnPointA : lineView.SpawnPointB;
            var outpostView = GetNearestOutpost(lineView, spawnPoint);
            var group = new CharacterGroup(
                GetGroupSlotSpacing(characterSettings),
                spawnPoint.position,
                spawnPoint.forward);
            for (var characterIndex = 0; characterIndex < characterSettings.Count; characterIndex++)
            {
                var settings = characterSettings[characterIndex];
                var characterView = Object.Instantiate(
                    settings.CharacterPrefab,
                    group.GetSpawnPosition(characterIndex),
                    spawnPoint.rotation,
                    levelView.CharactersRoot);
                _characterCombatSystem.RegisterCharacter(
                    characterView,
                    lineView,
                    settings.BaseMovementSpeed,
                    group,
                    characterIndex,
                    outpostView,
                    team,
                    _stateSettings.StartSettings,
                    settings,
                    _matchModel,
                    team == OutpostTeam.Player
                        ? lineView.SpawnPointB
                        : lineView.SpawnPointA,
                    team == OutpostTeam.Player ? OutpostTeam.Enemy : OutpostTeam.Player);
            }
        }

        private static float GetGroupSlotSpacing(IReadOnlyList<CharacterSettings> characterSettings)
        {
            var largestRadius = 0f;
            foreach (var settings in characterSettings)
            {
                largestRadius = Mathf.Max(largestRadius, settings.CharacterPrefab.NavigationAgent.radius);
            }

            var slotSpacing = largestRadius * 2.5f;

            return slotSpacing;
        }

        private static OutpostView GetNearestOutpost(LineView lineView, Transform origin)
        {
            var nearestOutpost = lineView.Outposts[0];
            var minimumDistance = Vector3.SqrMagnitude(
                origin.position - nearestOutpost.CapturePoint.position);
            foreach (var outpostView in lineView.Outposts)
            {
                var distance = Vector3.SqrMagnitude(origin.position - outpostView.CapturePoint.position);
                if (distance >= minimumDistance)
                {
                    continue;
                }

                nearestOutpost = outpostView;
                minimumDistance = distance;
            }

            return nearestOutpost;
        }

        private void RestartWaveTimer()
        {
            var startSettings = _stateSettings.StartSettings;
            _remainingWaveTime = _isFirstWave
                ? startSettings.FirstWaveCooldown
                : startSettings.WaveCooldown;
            _isFirstWave = false;
            _characterWaveModel.SetSecondsUntilNextWave(Mathf.CeilToInt(_remainingWaveTime));
            _characterWaveModel.SetSpawnLocked(false);
        }

        private sealed class PendingCharacterSpawn
        {
            public LevelView LevelView { get; }

            public LineCode LineCode { get; }

            public OutpostTeam Team { get; }

            public IReadOnlyList<CharacterSettings> CharacterSettings { get; }

            public PendingCharacterSpawn(
                LevelView levelView,
                LineCode lineCode,
                OutpostTeam team,
                IReadOnlyList<CharacterSettings> characterSettings)
            {
                LevelView = levelView;
                LineCode = lineCode;
                Team = team;
                CharacterSettings = characterSettings;
            }
        }
    }
}
