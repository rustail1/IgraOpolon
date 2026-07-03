using R3;

namespace Game
{
    public interface ICoreLevelsModel
    {
        ReadOnlyReactiveProperty<ILevelModel> Level { get; }

        ILevelModel InitializeLevel(LevelSettings levelSettings);
    }
}
