using System;
using System.Collections.Generic;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class OutpostsModel : GameModel<OutpostsModel, IOutpostsModel>, IOutpostsModel
    {
        private readonly Dictionary<OutpostView, OutpostState> _outpostStates = new();

        public event Action<OutpostView, OutpostTeam> Captured;

        public void Initialize(LevelView levelView, OutpostsSettings settings)
        {
            _outpostStates.Clear();
            foreach (LineCode lineCode in Enum.GetValues(typeof(LineCode)))
            {
                var lineView = levelView.GetLine(lineCode);
                foreach (var outpostView in lineView.Outposts)
                {
                    var outpostState = new OutpostState(outpostView, settings.BaseCaptureDuration);
                    _outpostStates.Add(outpostView, outpostState);
                    outpostView.SetState(OutpostTeam.None, OutpostTeam.None, 0f, 0f);
                }
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (var outpostState in _outpostStates.Values)
            {
                RemoveDestroyedCharacters(outpostState);
                var capturingTeam = GetCapturingTeam(outpostState);
                if (capturingTeam == OutpostTeam.None || capturingTeam == outpostState.Owner)
                {
                    ResetCapture(outpostState);

                    continue;
                }

                if (capturingTeam != outpostState.CapturingTeam)
                {
                    outpostState.CapturingTeam = capturingTeam;
                    outpostState.CaptureElapsedTime = 0f;
                }

                outpostState.CaptureElapsedTime = UnityEngine.Mathf.Min(
                    outpostState.CaptureElapsedTime + deltaTime,
                    outpostState.CaptureDuration);
                outpostState.View.SetState(
                    outpostState.Owner,
                    outpostState.CapturingTeam,
                    outpostState.CaptureElapsedTime,
                    outpostState.CaptureElapsedTime / outpostState.CaptureDuration);
                if (outpostState.CaptureElapsedTime < outpostState.CaptureDuration)
                {
                    continue;
                }

                outpostState.Owner = capturingTeam;
                outpostState.View.SetState(outpostState.Owner, OutpostTeam.None, 0f, 0f);
                outpostState.View.NotifyCaptureCompleted();
                Captured?.Invoke(outpostState.View, capturingTeam);
                ResetCapture(outpostState);
            }
        }

        public void AddCharacter(OutpostView outpostView, CharacterView characterView)
        {
            var outpostState = _outpostStates[outpostView];
            if (outpostState.Characters.Contains(characterView))
            {
                return;
            }

            outpostState.Characters.Add(characterView);
        }

        public void RemoveCharacter(OutpostView outpostView, CharacterView characterView)
        {
            _outpostStates[outpostView].Characters.Remove(characterView);
        }

        public IReadOnlyList<CharacterView> TakeCharacters(OutpostView outpostView, OutpostTeam team)
        {
            var outpostState = _outpostStates[outpostView];
            var characters = outpostState.Characters.FindAll(characterView => characterView.Team == team);
            outpostState.Characters.RemoveAll(characterView => characterView.Team == team);

            return characters;
        }

        public OutpostTeam GetOwner(OutpostView outpostView)
        {
            return _outpostStates[outpostView].Owner;
        }

        private static void RemoveDestroyedCharacters(OutpostState outpostState)
        {
            outpostState.Characters.RemoveAll(characterView => !characterView);
        }

        private static OutpostTeam GetCapturingTeam(OutpostState outpostState)
        {
            var hasPlayerCharacter = false;
            var hasEnemyCharacter = false;
            foreach (var characterView in outpostState.Characters)
            {
                if (characterView.Team == OutpostTeam.Player)
                {
                    hasPlayerCharacter = true;
                }
                else if (characterView.Team == OutpostTeam.Enemy)
                {
                    hasEnemyCharacter = true;
                }
            }

            if (hasPlayerCharacter == hasEnemyCharacter)
            {
                return OutpostTeam.None;
            }

            return hasPlayerCharacter ? OutpostTeam.Player : OutpostTeam.Enemy;
        }

        private static void ResetCapture(OutpostState outpostState)
        {
            outpostState.CapturingTeam = OutpostTeam.None;
            outpostState.CaptureElapsedTime = 0f;
            outpostState.View.SetState(outpostState.Owner, OutpostTeam.None, 0f, 0f);
        }

        private sealed class OutpostState
        {
            public OutpostView View { get; }

            public List<CharacterView> Characters { get; } = new();

            public float CaptureDuration { get; }

            public OutpostTeam Owner { get; set; }

            public OutpostTeam CapturingTeam { get; set; }

            public float CaptureElapsedTime { get; set; }

            public OutpostState(OutpostView view, float captureDuration)
            {
                View = view;
                CaptureDuration = captureDuration;
            }
        }
    }
}
