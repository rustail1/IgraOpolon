
namespace Game
{
    public interface ILevelModel
    {
        LevelSettings Settings { get; }

        LevelView View { get; }
    }
}
