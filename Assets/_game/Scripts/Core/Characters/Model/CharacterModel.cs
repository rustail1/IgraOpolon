using System;
using UnityEngine;

namespace Game
{
    public sealed class CharacterModel
    {
        public CharacterView View { get; }

        public LineView LineView { get; }

        public StartSettings Settings { get; }

        public CharacterSettings CharacterSettings { get; }

        public IMatchModel MatchModel { get; }

        public Transform ObjectiveAttackPoint { get; }

        public OutpostTeam ObjectiveTeam { get; }

        public OutpostTeam Team { get; }

        public float MovementSpeed { get; }

        public CharacterNavigation Navigation { get; }

        public CharacterGroup Group { get; private set; }

        public int GroupSlotIndex { get; private set; }

        public int Health { get; set; }

        public OutpostView DestinationOutpost { get; set; }

        public CharacterModel TargetCharacter { get; set; }

        public bool IsWaitingAtOutpost { get; set; }

        public bool IsAtObjective { get; set; }

        public bool IsAttacking { get; set; }

        public bool HasAppliedAttackDamage { get; set; }

        public float AttackHitTime { get; set; }

        public float AttackEndTime { get; set; }

        public bool IsDying { get; set; }

        public float DeathTime { get; set; }

        public bool IsAlive { get; set; } = true;

        public Action AttackHitHandler { get; set; }

        public Action AttackEndedHandler { get; set; }

        public CharacterModel(
            CharacterView view,
            LineView lineView,
            float movementSpeed,
            CharacterNavigation navigation,
            CharacterGroup group,
            int groupSlotIndex,
            OutpostView destinationOutpost,
            OutpostTeam team,
            StartSettings settings,
            CharacterSettings characterSettings,
            IMatchModel matchModel,
            Transform objectiveAttackPoint,
            OutpostTeam objectiveTeam)
        {
            View = view;
            LineView = lineView;
            MovementSpeed = movementSpeed;
            Navigation = navigation;
            Group = group;
            GroupSlotIndex = groupSlotIndex;
            DestinationOutpost = destinationOutpost;
            Team = team;
            Settings = settings;
            CharacterSettings = characterSettings;
            MatchModel = matchModel;
            ObjectiveAttackPoint = objectiveAttackPoint;
            ObjectiveTeam = objectiveTeam;
            Health = characterSettings.Health;
        }

        public void AssignGroup(CharacterGroup group, int groupSlotIndex)
        {
            Group = group;
            GroupSlotIndex = groupSlotIndex;
        }
    }
}
