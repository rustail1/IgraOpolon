using R3;

namespace Game
{
    public interface ICharacterWaveModel
    {
        ReadOnlyReactiveProperty<int> SecondsUntilNextWave { get; }

        ReadOnlyReactiveProperty<bool> IsSpawnLocked { get; }

        void SetSecondsUntilNextWave(int seconds);

        void SetSpawnLocked(bool isSpawnLocked);
    }
}
