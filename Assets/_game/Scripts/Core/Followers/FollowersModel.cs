using R3;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class FollowersModel : GameModel<FollowersModel, IFollowersModel>, IFollowersModel
    {
        private CoreStateSettings _stateSettings;

        public ReactiveProperty<int> Total { get; } = new();
        public ReactiveProperty<int> Available { get; } = new();
        public ReactiveProperty<int> Mining { get; } = new();
        public ReactiveProperty<int> Summoning { get; } = new();
        public ReactiveProperty<int> BuildingLocked { get; } = new();

        [Inject]
        private void Construct(CoreStateSettings stateSettings)
        {
            _stateSettings = stateSettings;
        }

        protected override void OnInitialize()
        {
            Total.Value = _stateSettings.StartSettings.StartingFollowers;
            Available.Value = Total.Value;
        }

        public void AddFollowers(int amount)
        {
            Total.Value += amount;
            Available.Value += amount;
        }

        public bool TryAssignMining(int amount)
        {
            return TryAssign(amount, Mining);
        }

        public bool TryAssignSummoning(int amount)
        {
            return TryAssign(amount, Summoning);
        }

        public bool TryAssignBuilding(int amount)
        {
            return TryAssign(amount, BuildingLocked);
        }

        public void ReleaseMining(int amount)
        {
            Release(amount, Mining);
        }

        public void ReleaseSummoning(int amount)
        {
            Release(amount, Summoning);
        }

        public void ReleaseBuilding(int amount)
        {
            Release(amount, BuildingLocked);
        }

        private bool TryAssign(int amount, ReactiveProperty<int> target)
        {
            if (Available.Value < amount)
                return false;

            Available.Value -= amount;
            target.Value += amount;

            return true;
        }

        private void Release(int amount, ReactiveProperty<int> target)
        {
            amount = System.Math.Min(amount, target.Value);
            target.Value -= amount;
            Available.Value += amount;
        }
    }
}