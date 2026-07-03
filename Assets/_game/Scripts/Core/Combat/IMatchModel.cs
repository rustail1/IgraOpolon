using R3;

namespace Game
{
    public interface IMatchModel
    {
        ReadOnlyReactiveProperty<int> SecondsRemaining { get; }

        ReadOnlyReactiveProperty<int> PlayerBaseHealth { get; }

        ReadOnlyReactiveProperty<int> EnemyAltarHealth { get; }

        ReadOnlyReactiveProperty<MatchState> State { get; }

        bool IsPlaying { get; }

        void Tick(float deltaTime);

        void ApplyDamage(OutpostTeam targetTeam, int damage);
    }
}
