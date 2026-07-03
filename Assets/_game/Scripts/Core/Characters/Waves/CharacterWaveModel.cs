using R3;

namespace Game
{
    [GameModel(nameof(GameStateCode.Core))]
    public sealed class CharacterWaveModel : GameModel<CharacterWaveModel, ICharacterWaveModel>, ICharacterWaveModel
    {
        private readonly ReactiveProperty<int> _secondsUntilNextWave = new();
        private readonly ReactiveProperty<bool> _isSpawnLocked = new();

        public ReadOnlyReactiveProperty<int> SecondsUntilNextWave => _secondsUntilNextWave;

        public ReadOnlyReactiveProperty<bool> IsSpawnLocked => _isSpawnLocked;

        public void SetSecondsUntilNextWave(int seconds)
        {
            _secondsUntilNextWave.Value = seconds;
        }

        public void SetSpawnLocked(bool isSpawnLocked)
        {
            _isSpawnLocked.Value = isSpawnLocked;
        }
    }
}
