namespace Game
{
    public interface ICharacterCombatSystem
    {
        void RegisterCharacter(
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
            UnityEngine.Transform objectiveAttackPoint,
            OutpostTeam objectiveTeam);

        void ContinueMovement(CharacterView characterView, OutpostView destinationOutpost);
    }
}
