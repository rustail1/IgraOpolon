namespace Game
{
    public class LevelModel : ILevelModel
    {
        public LevelModel( LevelView view, LevelSettings settings)
        {
            Settings = settings;
            View = view;
        }

        public LevelSettings Settings { get; }

        public LevelView View { get; }
    }
}
