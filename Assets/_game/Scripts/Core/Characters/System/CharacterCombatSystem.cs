using System.Collections.Generic;
using UnityEngine;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class CharacterCombatSystem : SyncLifetime, IGameSystem, IUpdate, ICharacterCombatSystem
    {
        private const float CorpseLifetime = 15f;
        private const float DestinationStoppingDistance = 0.05f;
        private const float ExitStoppingDistance = 0.6f;
        private const float GroupMergeCheckInterval = 0.25f;
        private const float GroupMergeDistance = 1.4f;
        private const float AttackSlotAngle = 35f;
        private const float AttackPointDistancePadding = 0.05f;
        private const float AttackHitTimeRatio = 0.5f;
        private const int MaximumAvoidancePriority = 99;

        private readonly List<CharacterModel> _characterModels = new();
        private readonly List<CharacterModel> _groupRepresentatives = new();
        private IOutpostsModel _outpostsModel;
        private ICoreLevelsModel _levelsModel;
        private IMatchModel _matchModel;
        private Camera _camera;
        private BattleUISettings _battleUISettings;
        private StartSettings _startSettings;
        private float _remainingGroupMergeCheckTime;

        [VContainer.Inject]
        private void Construct(
            IOutpostsModel outpostsModel,
            ICoreLevelsModel levelsModel,
            IMatchModel matchModel,
            Camera camera,
            CoreStateSettings stateSettings)
        {
            _outpostsModel = outpostsModel;
            _levelsModel = levelsModel;
            _matchModel = matchModel;
            _camera = camera;
            _battleUISettings = stateSettings.BattleUISettings;
            _startSettings = stateSettings.StartSettings;
        }
        public static CharacterCombatSystem Instance { get; private set; }

        protected override void OnInitialize()
        {
            Instance = this;
        }
        public void OnUpdate()
        {
            UpdateBaseHealthBars();
            RemoveExpiredCorpses();
            TickGroupMerges();
            for (var index = _characterModels.Count - 1; index >= 0; index--)
            {
                var characterModel = _characterModels[index];
                if (!characterModel.IsAlive || !characterModel.MatchModel.IsPlaying)
                {
                    continue;
                }

                UpdateCharacter(characterModel);
            }
        }
        public void ApplyDamage(CharacterView target, int damage)
        {
            if (target == null)
                return;

            var characterModel = GetCharacterModel(target);

            if (characterModel == null)
                return;

            ApplyDamage(characterModel, damage);
        }
        private void TickGroupMerges()
        {
            _remainingGroupMergeCheckTime -= Time.deltaTime;
            if (_remainingGroupMergeCheckTime > 0f)
            {
                return;
            }

            _remainingGroupMergeCheckTime = GroupMergeCheckInterval;
            _groupRepresentatives.Clear();
            foreach (var characterModel in _characterModels)
            {
                if (!characterModel.IsAlive || !characterModel.MatchModel.IsPlaying ||
                    HasGroupRepresentative(characterModel.Group))
                {
                    continue;
                }

                _groupRepresentatives.Add(characterModel);
            }

            for (var firstIndex = 0; firstIndex < _groupRepresentatives.Count; firstIndex++)
            {
                for (var secondIndex = firstIndex + 1; secondIndex < _groupRepresentatives.Count; secondIndex++)
                {
                    var firstGroupRepresentative = _groupRepresentatives[firstIndex];
                    var secondGroupRepresentative = _groupRepresentatives[secondIndex];
                    if (!CanMergeGroups(firstGroupRepresentative, secondGroupRepresentative))
                    {
                        continue;
                    }

                    MergeGroups(firstGroupRepresentative, secondGroupRepresentative);

                    return;
                }
            }
        }

        private bool HasGroupRepresentative(CharacterGroup group)
        {
            foreach (var groupRepresentative in _groupRepresentatives)
            {
                if (groupRepresentative.Group == group)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanMergeGroups(
            CharacterModel firstGroupRepresentative,
            CharacterModel secondGroupRepresentative)
        {
            if (firstGroupRepresentative.Group == secondGroupRepresentative.Group ||
                firstGroupRepresentative.Team != secondGroupRepresentative.Team ||
                firstGroupRepresentative.LineView != secondGroupRepresentative.LineView)
            {
                return false;
            }

            var canMerge = AreGroupsClose(
                firstGroupRepresentative.Group,
                secondGroupRepresentative.Group);

            return canMerge;
        }

        private bool AreGroupsClose(CharacterGroup firstGroup, CharacterGroup secondGroup)
        {
            var maximumMergeDistance = GroupMergeDistance * GroupMergeDistance;
            foreach (var firstCharacterModel in _characterModels)
            {
                if (!firstCharacterModel.IsAlive || firstCharacterModel.Group != firstGroup)
                {
                    continue;
                }

                foreach (var secondCharacterModel in _characterModels)
                {
                    if (!secondCharacterModel.IsAlive || secondCharacterModel.Group != secondGroup)
                    {
                        continue;
                    }

                    var distance = GetHorizontalSqrDistance(
                        firstCharacterModel.View.RootTransform.position,
                        secondCharacterModel.View.RootTransform.position);
                    if (distance <= maximumMergeDistance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void MergeGroups(
            CharacterModel firstGroupRepresentative,
            CharacterModel secondGroupRepresentative)
        {
            var targetGroupRepresentative = GetMergeTarget(
                firstGroupRepresentative,
                secondGroupRepresentative);
            var sourceGroupRepresentative = targetGroupRepresentative == firstGroupRepresentative
                ? secondGroupRepresentative
                : firstGroupRepresentative;
            var waitingOutpost = GetGroupWaitingOutpost(targetGroupRepresentative.Group) ??
                GetGroupWaitingOutpost(sourceGroupRepresentative.Group);
            var destinationOutpost = targetGroupRepresentative.DestinationOutpost;
            var targetGroup = targetGroupRepresentative.Group;
            var sourceGroup = sourceGroupRepresentative.Group;
            ReassignMergedGroupSlots(targetGroup, sourceGroup);

            if (waitingOutpost != null)
            {
                targetGroupRepresentative.DestinationOutpost = waitingOutpost;
                StopGroupAtOutpost(targetGroupRepresentative);

                return;
            }

            foreach (var characterModel in _characterModels)
            {
                if (!characterModel.IsAlive || characterModel.Group != targetGroup)
                {
                    continue;
                }

                ContinueMovement(characterModel.View, destinationOutpost);
            }
        }

        private void ReassignMergedGroupSlots(CharacterGroup targetGroup, CharacterGroup sourceGroup)
        {
            var groupSlotIndex = 0;
            foreach (var characterModel in _characterModels)
            {
                if (!characterModel.IsAlive || characterModel.Group != targetGroup)
                {
                    continue;
                }

                characterModel.AssignGroup(targetGroup, groupSlotIndex);
                groupSlotIndex++;
            }

            foreach (var characterModel in _characterModels)
            {
                if (!characterModel.IsAlive || characterModel.Group != sourceGroup)
                {
                    continue;
                }

                characterModel.AssignGroup(targetGroup, groupSlotIndex);
                groupSlotIndex++;
            }
        }

        private CharacterModel GetMergeTarget(
            CharacterModel firstGroupRepresentative,
            CharacterModel secondGroupRepresentative)
        {
            if (GetGroupWaitingOutpost(firstGroupRepresentative.Group) != null)
            {
                return firstGroupRepresentative;
            }

            if (GetGroupWaitingOutpost(secondGroupRepresentative.Group) != null)
            {
                return secondGroupRepresentative;
            }

            if (IsAheadOf(firstGroupRepresentative, secondGroupRepresentative))
            {
                return firstGroupRepresentative;
            }

            return secondGroupRepresentative;
        }

        private OutpostView GetGroupWaitingOutpost(CharacterGroup group)
        {
            foreach (var characterModel in _characterModels)
            {
                if (!characterModel.IsAlive || characterModel.Group != group ||
                    !characterModel.IsWaitingAtOutpost)
                {
                    continue;
                }

                return characterModel.DestinationOutpost;
            }

            return null;
        }

        private static bool IsAheadOf(
            CharacterModel firstGroupRepresentative,
            CharacterModel secondGroupRepresentative)
        {
            var firstDestinationOrder = GetDestinationOrder(firstGroupRepresentative);
            var secondDestinationOrder = GetDestinationOrder(secondGroupRepresentative);
            if (firstGroupRepresentative.Team == OutpostTeam.Enemy)
            {
                return firstDestinationOrder >= secondDestinationOrder;
            }

            return firstDestinationOrder <= secondDestinationOrder;
        }

        private static int GetDestinationOrder(CharacterModel characterModel)
        {
            if (characterModel.DestinationOutpost == null)
            {
                var exitOrder = characterModel.Team == OutpostTeam.Enemy
                    ? characterModel.LineView.Outposts.Length
                    : -1;

                return exitOrder;
            }

            var outpostIndex = System.Array.IndexOf(
                characterModel.LineView.Outposts,
                characterModel.DestinationOutpost);

            return outpostIndex;
        }

        public void RegisterCharacter(
            CharacterView characterView,
            LineView lineView,
            float movementSpeed,
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
            var navigation = new CharacterNavigation(
                characterView,
                movementSpeed,
                GetAvoidancePriority());
            var characterModel = new CharacterModel(
                characterView,
                lineView,
                movementSpeed,
                navigation,
                group,
                groupSlotIndex,
                destinationOutpost,
                team,
                settings,
                characterSettings,
                matchModel,
                objectiveAttackPoint,
                objectiveTeam);
            characterView.Initialize(characterModel);
            characterModel.AttackHitHandler = () => ApplyAttack(characterModel);
            characterModel.AttackEndedHandler = () => CompleteAttack(characterModel);
            characterView.AttackHit += characterModel.AttackHitHandler;
            characterView.AttackEnded += characterModel.AttackEndedHandler;
            _characterModels.Add(characterModel);
            UpdateHealthBar(characterModel);
            ContinueMovement(characterView, destinationOutpost);
        }

        public void ContinueMovement(CharacterView characterView, OutpostView destinationOutpost)
        {
            var characterModel = GetCharacterModel(characterView);
            if (characterModel == null || !characterModel.IsAlive)
            {
                return;
            }

            characterModel.DestinationOutpost = destinationOutpost;
            characterModel.IsWaitingAtOutpost = false;
            characterModel.IsAtObjective = false;
            characterModel.View.ResetAnimationSpeed();
            characterModel.Navigation.MoveTo(
                GetMovementDestination(characterModel),
                GetMovementStoppingDistance(characterModel));
            UpdateMovementState(characterModel);
        }

        private void UpdateCharacter(CharacterModel characterModel)
        {
            if (TickAttack(characterModel) || UpdateCombat(characterModel) || characterModel.IsWaitingAtOutpost)
            {
                return;
            }

            characterModel.Navigation.MoveTo(
                GetMovementDestination(characterModel),
                GetMovementStoppingDistance(characterModel));
            if (characterModel.Navigation.HasReachedDestination())
            {
                ReachDestination(characterModel);

                return;
            }

            UpdateMovementState(characterModel);
        }

        private bool UpdateCombat(CharacterModel characterModel)
        {
            var nearestEnemy = GetNearestEnemy(characterModel);
            characterModel.TargetCharacter = nearestEnemy;
            if (nearestEnemy != null)
            {
                if (IsWithinAttackRange(characterModel, nearestEnemy))
                {
                    characterModel.Navigation.Stop();
                    TryStartAttack(characterModel);

                    return true;
                }

                characterModel.Navigation.MoveTo(
                    GetAttackDestination(characterModel, nearestEnemy),
                    DestinationStoppingDistance);
                UpdateMovementState(characterModel);

                return true;
            }

            if (!characterModel.IsAtObjective || IsObjectiveDestroyed(characterModel))
            {
                return false;
            }

            var objectivePosition = characterModel.ObjectiveAttackPoint.position;
            if (GetHorizontalDistance(characterModel.View.RootTransform.position, objectivePosition) <=
                characterModel.Settings.UnitAttackRange)
            {
                characterModel.Navigation.Stop();
                TryStartAttack(characterModel);

                return true;
            }

            characterModel.Navigation.MoveTo(objectivePosition, characterModel.Settings.UnitAttackRange);
            UpdateMovementState(characterModel);

            return true;
        }

        private CharacterModel GetNearestEnemy(CharacterModel characterModel)
        {
            CharacterModel nearestEnemy = null;
            var nearestEnemyDistance = characterModel.Settings.UnitAggroRange * characterModel.Settings.UnitAggroRange;
            foreach (var otherCharacterModel in _characterModels)
            {
                if (!otherCharacterModel.IsAlive || characterModel.Team == otherCharacterModel.Team ||
                    characterModel.LineView != otherCharacterModel.LineView)
                {
                    continue;
                }

                var positionDifference = otherCharacterModel.View.RootTransform.position -
                    characterModel.View.RootTransform.position;
                positionDifference.y = 0f;
                var enemyDistance = positionDifference.sqrMagnitude;
                if (enemyDistance > nearestEnemyDistance)
                {
                    continue;
                }

                nearestEnemy = otherCharacterModel;
                nearestEnemyDistance = enemyDistance;
            }

            return nearestEnemy;
        }

        private void ReachDestination(CharacterModel characterModel)
        {
            characterModel.Navigation.Stop();
            if (characterModel.DestinationOutpost == null)
            {
                ReachObjective(characterModel);

                return;
            }

            if (_outpostsModel.GetOwner(characterModel.DestinationOutpost) == characterModel.Team)
            {
                ContinueMovement(
                    characterModel.View,
                    GetNextOutpost(characterModel.LineView, characterModel.DestinationOutpost, characterModel.Team));

                return;
            }

            StopGroupAtOutpost(characterModel);
        }

        private void ReachObjective(CharacterModel characterModel)
        {
            characterModel.IsAtObjective = true;
            characterModel.View.SetMoving(false);
        }

        private void StopGroupAtOutpost(CharacterModel characterModel)
        {
            foreach (var groupCharacterModel in _characterModels)
            {
                if (!groupCharacterModel.IsAlive || groupCharacterModel.Group != characterModel.Group)
                {
                    continue;
                }

                groupCharacterModel.DestinationOutpost = characterModel.DestinationOutpost;
                groupCharacterModel.IsWaitingAtOutpost = true;
                groupCharacterModel.Navigation.Stop();
                groupCharacterModel.View.ResetAnimationSpeed();
                groupCharacterModel.View.SetMoving(false);
                _outpostsModel.AddCharacter(
                    characterModel.DestinationOutpost,
                    groupCharacterModel.View);
            }
        }

        private void TryStartAttack(CharacterModel characterModel)
        {
            if (characterModel.IsAttacking)
            {
                return;
            }

            characterModel.IsAttacking = true;
            characterModel.HasAppliedAttackDamage = false;
            var attackDuration = GetAttackDuration(characterModel);
            characterModel.AttackEndTime = Time.time + attackDuration;
            characterModel.AttackHitTime = Time.time + attackDuration * AttackHitTimeRatio;
            characterModel.View.SetMoving(false);
            FaceAttackTarget(characterModel);
            characterModel.View.PlayAttack();
        }

        private bool TickAttack(CharacterModel characterModel)
        {
            if (!characterModel.IsAttacking)
            {
                return false;
            }

            if (!characterModel.HasAppliedAttackDamage && Time.time >= characterModel.AttackHitTime)
            {
                ApplyAttack(characterModel);
            }

            if (Time.time < characterModel.AttackEndTime)
            {
                return true;
            }

            CompleteAttack(characterModel);

            return characterModel.IsAttacking;
        }

        private static void FaceAttackTarget(CharacterModel characterModel)
        {
            var targetPosition = characterModel.TargetCharacter != null && characterModel.TargetCharacter.IsAlive
                ? characterModel.TargetCharacter.View.RootTransform.position
                : characterModel.ObjectiveAttackPoint.position;
            characterModel.Navigation.ForceFacePosition(targetPosition);
        }

        private void ApplyAttack(CharacterModel characterModel)
        {
            if (!characterModel.IsAlive || !characterModel.IsAttacking || characterModel.HasAppliedAttackDamage)
            {
                return;
            }

            characterModel.HasAppliedAttackDamage = true;
            FaceAttackTarget(characterModel);
            if (characterModel.TargetCharacter != null && characterModel.TargetCharacter.IsAlive &&
                IsWithinAttackRange(characterModel, characterModel.TargetCharacter))
            {
                ApplyDamage(characterModel.TargetCharacter, characterModel.CharacterSettings.Attack);

                return;
            }

            if (characterModel.IsAtObjective && !IsObjectiveDestroyed(characterModel) &&
                GetHorizontalDistance(
                    characterModel.View.RootTransform.position,
                    characterModel.ObjectiveAttackPoint.position) <= characterModel.Settings.UnitAttackRange)
            {
                characterModel.MatchModel.ApplyDamage(
                    characterModel.ObjectiveTeam,
                    characterModel.CharacterSettings.Attack);
            }
        }

        private void CompleteAttack(CharacterModel characterModel)
        {
            if (!characterModel.IsAttacking)
            {
                return;
            }

            if (!characterModel.HasAppliedAttackDamage)
            {
                ApplyAttack(characterModel);
            }

            characterModel.IsAttacking = false;
            characterModel.View.ResetAnimationSpeed();
            if (!characterModel.IsAlive)
            {
                return;
            }

            if (characterModel.TargetCharacter != null && characterModel.TargetCharacter.IsAlive &&
                IsWithinAttackRange(characterModel, characterModel.TargetCharacter))
            {
                TryStartAttack(characterModel);

                return;
            }

            if (characterModel.IsAtObjective && !IsObjectiveDestroyed(characterModel) &&
                GetHorizontalDistance(
                    characterModel.View.RootTransform.position,
                    characterModel.ObjectiveAttackPoint.position) <= characterModel.Settings.UnitAttackRange)
            {
                TryStartAttack(characterModel);
            }
        }

        private void ApplyDamage(CharacterModel characterModel, int damage)
        {
            if (!characterModel.IsAlive)
            {
                return;
            }

            var resultingDamage = Mathf.Max(damage - characterModel.CharacterSettings.Defense, 0);
            characterModel.Health -= resultingDamage;
            UpdateHealthBar(characterModel);
            if (characterModel.Health > 0)
            {
                return;
            }

            KillCharacter(characterModel);
        }

        private void KillCharacter(CharacterModel characterModel)
        {
            if (!characterModel.IsAlive)
            {
                return;
            }

            characterModel.IsAlive = false;
            characterModel.IsDying = true;
            characterModel.DeathTime = Time.time;
            characterModel.Health = 0;
            characterModel.Navigation.Disable();
            characterModel.View.SetMoving(false);
            characterModel.View.PlayDying();
            UpdateHealthBar(characterModel);
            if (characterModel.DestinationOutpost != null)
            {
                _outpostsModel.RemoveCharacter(characterModel.DestinationOutpost, characterModel.View);
            }
        }

        private void DestroyCharacter(CharacterModel characterModel)
        {
            characterModel.View.AttackHit -= characterModel.AttackHitHandler;
            characterModel.View.AttackEnded -= characterModel.AttackEndedHandler;
            _characterModels.Remove(characterModel);
            Object.Destroy(characterModel.View.gameObject);
        }

        private static Vector3 GetMovementDestination(CharacterModel characterModel)
        {
            if (characterModel.DestinationOutpost == null)
            {
                var exitDestination = characterModel.ObjectiveAttackPoint.position;

                return exitDestination;
            }

            var destination = characterModel.Group.GetDestinationPosition(
                characterModel.GroupSlotIndex,
                characterModel.DestinationOutpost.CapturePoint.position);

            return destination;
        }

        private static float GetMovementStoppingDistance(CharacterModel characterModel)
        {
            if (characterModel.DestinationOutpost == null)
            {
                var stoppingDistance = ExitStoppingDistance;

                return stoppingDistance;
            }

            return DestinationStoppingDistance;
        }

        private void RemoveExpiredCorpses()
        {
            for (var index = _characterModels.Count - 1; index >= 0; index--)
            {
                var characterModel = _characterModels[index];
                if (characterModel.IsAlive || !characterModel.IsDying ||
                    Time.time < characterModel.DeathTime + CorpseLifetime)
                {
                    continue;
                }

                DestroyCharacter(characterModel);
            }
        }

        private static float GetHorizontalDistance(Vector3 firstPosition, Vector3 secondPosition)
        {
            var difference = firstPosition - secondPosition;
            difference.y = 0f;

            return difference.magnitude;
        }

        private static float GetHorizontalSqrDistance(Vector3 firstPosition, Vector3 secondPosition)
        {
            var difference = firstPosition - secondPosition;
            difference.y = 0f;

            return difference.sqrMagnitude;
        }

        private static void UpdateMovementState(CharacterModel characterModel)
        {
            characterModel.Navigation.UpdateFacing();
            characterModel.View.SetMoving(characterModel.Navigation.IsMoving);
        }

        private static float GetAttackStoppingDistance(
            CharacterModel characterModel,
            CharacterModel targetCharacterModel)
        {
            var stoppingDistance = characterModel.Settings.UnitAttackRange +
                characterModel.Navigation.Radius + targetCharacterModel.Navigation.Radius;

            return stoppingDistance;
        }

        private static Vector3 GetAttackDestination(
            CharacterModel characterModel,
            CharacterModel targetCharacterModel)
        {
            var targetPosition = targetCharacterModel.View.RootTransform.position;
            var direction = characterModel.View.RootTransform.position - targetPosition;
            direction.y = 0f;
            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                direction = targetPosition - characterModel.ObjectiveAttackPoint.position;
                direction.y = 0f;
            }

            if (direction.sqrMagnitude <= Mathf.Epsilon)
            {
                direction = characterModel.View.RootTransform.forward;
                direction.y = 0f;
            }

            var attackDirection = Quaternion.AngleAxis(
                GetAttackSlotAngle(characterModel.GroupSlotIndex),
                Vector3.up) * direction.normalized;
            var attackDistance = Mathf.Max(
                GetAttackStoppingDistance(characterModel, targetCharacterModel) - AttackPointDistancePadding,
                characterModel.Navigation.Radius + targetCharacterModel.Navigation.Radius);
            var attackDestination = targetPosition + attackDirection * attackDistance;

            return attackDestination;
        }

        private static float GetAttackSlotAngle(int groupSlotIndex)
        {
            if (groupSlotIndex == 0)
            {
                return 0f;
            }

            var slotLayer = (groupSlotIndex + 1) / 2;
            var direction = groupSlotIndex % 2 == 0 ? -1f : 1f;
            var angle = slotLayer * AttackSlotAngle * direction;

            return angle;
        }

        private static bool IsWithinAttackRange(
            CharacterModel characterModel,
            CharacterModel targetCharacterModel)
        {
            var distance = GetHorizontalDistance(
                characterModel.View.RootTransform.position,
                targetCharacterModel.View.RootTransform.position);
            var isWithinRange = distance <= GetAttackStoppingDistance(characterModel, targetCharacterModel);

            return isWithinRange;
        }

        private static float GetAttackDuration(CharacterModel characterModel)
        {
            var attackDuration = 1f / characterModel.CharacterSettings.AttackSpeed;

            return attackDuration;
        }

        private static bool IsObjectiveDestroyed(CharacterModel characterModel)
        {
            if (characterModel.ObjectiveTeam == OutpostTeam.Player)
            {
                return characterModel.MatchModel.PlayerBaseHealth.CurrentValue == 0;
            }

            return characterModel.MatchModel.EnemyAltarHealth.CurrentValue == 0;
        }

        private void UpdateHealthBar(CharacterModel characterModel)
        {
            var color = characterModel.Team == OutpostTeam.Player
                ? new Color(0.16f, 0.52f, 1f)
                : new Color(0.9f, 0.2f, 0.2f);
            characterModel.View.SetHealthBar(
                _camera,
                characterModel.Health,
                characterModel.CharacterSettings.Health,
                color,
                _battleUISettings.HealthBarVerticalOffset);
        }

        private void UpdateBaseHealthBars()
        {
            var levelView = _levelsModel.Level.CurrentValue.View;
            var playerBaseColor = new Color(0.16f, 0.52f, 1f);
            var enemyAltarColor = new Color(0.9f, 0.2f, 0.2f);
            levelView.BaseConstructionView.SetHealthBar(
                _camera,
                _matchModel.PlayerBaseHealth.CurrentValue,
                _startSettings.PlayerBaseHealth,
                playerBaseColor,
                _battleUISettings.HealthBarVerticalOffset);
            levelView.EnemyBaseConstructionView.SetHealthBar(
                _camera,
                _matchModel.EnemyAltarHealth.CurrentValue,
                _startSettings.EnemyAltarHealth,
                enemyAltarColor,
                _battleUISettings.HealthBarVerticalOffset);
        }

        private CharacterModel GetCharacterModel(CharacterView characterView)
        {
            foreach (var characterModel in _characterModels)
            {
                if (characterModel.View == characterView)
                {
                    return characterModel;
                }
            }

            return null;
        }

        private int GetAvoidancePriority()
        {
            var priority = _characterModels.Count % MaximumAvoidancePriority + 1;

            return priority;
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
