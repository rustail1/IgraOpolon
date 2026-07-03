using UnityEngine;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class OutpostsSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private ICoreLevelsModel _levelsModel;
        private CoreStateSettings _stateSettings;
        private ICharacterCombatSystem _characterCombatSystem;
        private IOutpostsModel _outpostsModel;

        [Inject]
        private void Construct(
            ICoreLevelsModel levelsModel,
            CoreStateSettings stateSettings,
            ICharacterCombatSystem characterCombatSystem,
            IOutpostsModel outpostsModel)
        {
            _levelsModel = levelsModel;
            _stateSettings = stateSettings;
            _characterCombatSystem = characterCombatSystem;
            _outpostsModel = outpostsModel;
        }

        protected override void OnInitialize()
        {
            _outpostsModel.Initialize(_levelsModel.Level.CurrentValue.View, _stateSettings.OutpostsSettings);
            _outpostsModel.Captured += AdvanceCharacters;
        }

        protected override void OnRelease()
        {
            _outpostsModel.Captured -= AdvanceCharacters;
        }

        public void OnUpdate()
        {
            _outpostsModel.Tick(Time.deltaTime);
        }

        private void AdvanceCharacters(OutpostView outpostView, OutpostTeam team)
        {
            var lineView = GetLine(outpostView);
            var nextOutpost = GetNextOutpost(lineView, outpostView, team);
            foreach (var characterView in _outpostsModel.TakeCharacters(outpostView, team))
            {
                _characterCombatSystem.ContinueMovement(characterView, nextOutpost);
            }
        }

        private LineView GetLine(OutpostView outpostView)
        {
            foreach (LineCode lineCode in System.Enum.GetValues(typeof(LineCode)))
            {
                var lineView = _levelsModel.Level.CurrentValue.View.GetLine(lineCode);
                foreach (var candidateOutpostView in lineView.Outposts)
                {
                    if (candidateOutpostView == outpostView)
                    {
                        return lineView;
                    }
                }
            }

            Debug.LogError($"Line for {outpostView.name} was not found");

            return null;
        }

        private static OutpostView GetNextOutpost(
            LineView lineView,
            OutpostView outpostView,
            OutpostTeam team)
        {
            var outpostIndex = System.Array.IndexOf(lineView.Outposts, outpostView);
            var nextOutpostIndex = team == OutpostTeam.Enemy ? outpostIndex + 1 : outpostIndex - 1;
            if (nextOutpostIndex < 0 || nextOutpostIndex >= lineView.Outposts.Length)
            {
                return null;
            }

            return lineView.Outposts[nextOutpostIndex];
        }
    }
}
