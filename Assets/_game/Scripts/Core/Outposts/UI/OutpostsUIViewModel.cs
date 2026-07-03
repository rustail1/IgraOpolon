using Whaledevelop;
using Whaledevelop.UI;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace Game
{
    public sealed class OutpostsUIViewModel : SyncLifetime, IUIViewModel
    {
        private readonly List<OutpostView> _outposts = new();
        private ICoreLevelsModel _levelsModel;

        public Camera Camera { get; private set; }

        public Color TeamAColor { get; private set; }

        public Color TeamBColor { get; private set; }

        public IReadOnlyList<OutpostView> Outposts => _outposts;

        [Inject]
        private void Construct(
            ICoreLevelsModel levelsModel,
            Camera camera,
            CoreStateSettings stateSettings)
        {
            _levelsModel = levelsModel;
            Camera = camera;
            TeamAColor = stateSettings.StartSettings.TeamA.Color;
            TeamBColor = stateSettings.StartSettings.TeamB.Color;
        }

        public Color GetTeamColor(OutpostTeam team)
        {
            if (team == OutpostTeam.Player)
            {
                return TeamAColor;
            }

            if (team == OutpostTeam.Enemy)
            {
                return TeamBColor;
            }

            return Color.white;
        }

        protected override void OnInitialize()
        {
            foreach (LineCode lineCode in System.Enum.GetValues(typeof(LineCode)))
            {
                var lineView = _levelsModel.Level.CurrentValue.View.GetLine(lineCode);
                _outposts.AddRange(lineView.Outposts);
            }
        }

        protected override void OnRelease()
        {
            _outposts.Clear();
        }
    }
}
