using R3;

namespace Game
{
    public interface IFollowersModel
    {
        ReactiveProperty<int> Total { get; }

        ReactiveProperty<int> Available { get; }

        ReactiveProperty<int> Mining { get; }

        ReactiveProperty<int> Summoning { get; }

        ReactiveProperty<int> BuildingLocked { get; }

        bool TryAssignMining(int amount);

        bool TryAssignSummoning(int amount);

        bool TryAssignBuilding(int amount);

        void ReleaseMining(int amount);

        void ReleaseSummoning(int amount);

        void ReleaseBuilding(int amount);

        void AddFollowers(int amount);
    }
}