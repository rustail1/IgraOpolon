using System.Collections.Generic;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    public static class GameRuntimeDataComposition
    {
        public static void RegisterModels(IContainerBuilder builder)
        {

            return;
        }

        public static IReadOnlyList<IGameModel> ResolveModels(IObjectResolver resolver)
        {
            var models = new IGameModel[]
            {
            };

            return models;
        }

        public static void RegisterSystems(IContainerBuilder builder)
        {

            return;
        }

        public static IReadOnlyList<IGameSystem> ResolveSystems(IObjectResolver resolver)
        {
            var systems = new IGameSystem[]
            {
            };

            return systems;
        }
    }
}
