using System.Collections.Generic;
using VContainer;
using Whaledevelop.Systems;

namespace Game
{
    public static class CoreRuntimeDataComposition
    {
        public static void RegisterModels(IContainerBuilder builder)
        {
            builder.Register<global::Game.BaseModel>(Lifetime.Singleton).AsSelf().As<global::Game.IBaseModel>().As<IGameModel>();
            builder.Register<global::Game.CharacterWaveModel>(Lifetime.Singleton).AsSelf().As<global::Game.ICharacterWaveModel>().As<IGameModel>();
            builder.Register<global::Game.PlayerSquadsModel>(Lifetime.Singleton).AsSelf().As<global::Game.IPlayerSquadsModel>().As<IGameModel>();
            builder.Register<global::Game.ConstructionsModel>(Lifetime.Singleton).AsSelf().As<global::Game.IConstructionsModel>().As<IGameModel>();
            builder.Register<global::Game.CoreLevelsModel>(Lifetime.Singleton).AsSelf().As<global::Game.ICoreLevelsModel>().As<IGameModel>();
            builder.Register<global::Game.CurrenciesModel>(Lifetime.Singleton).AsSelf().As<global::Game.ICurrenciesModel>().As<IGameModel>();
            builder.Register<global::Game.MatchModel>(Lifetime.Singleton).AsSelf().As<global::Game.IMatchModel>().As<IGameModel>();
            builder.Register<global::Game.OutpostsModel>(Lifetime.Singleton).AsSelf().As<global::Game.IOutpostsModel>().As<IGameModel>();

            return;
        }

        public static IReadOnlyList<IGameModel> ResolveModels(IObjectResolver resolver)
        {
            var models = new IGameModel[]
            {
                resolver.Resolve<global::Game.BaseModel>(),
                resolver.Resolve<global::Game.CharacterWaveModel>(),
                resolver.Resolve<global::Game.PlayerSquadsModel>(),
                resolver.Resolve<global::Game.ConstructionsModel>(),
                resolver.Resolve<global::Game.CoreLevelsModel>(),
                resolver.Resolve<global::Game.CurrenciesModel>(),
                resolver.Resolve<global::Game.MatchModel>(),
                resolver.Resolve<global::Game.OutpostsModel>(),
            };

            return models;
        }

        public static void RegisterSystems(IContainerBuilder builder)
        {
            builder.Register<global::Game.BaseProductionSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();
            builder.Register<global::Game.CharacterSpawnSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();
            builder.Register<global::Game.CharacterCombatSystem>(Lifetime.Singleton).AsSelf().As<global::Game.ICharacterCombatSystem>().As<IGameSystem>();
            builder.Register<global::Game.ConstructionsBehaviourSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();
            builder.Register<global::Game.CoreFreeRoamCameraSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();
            builder.Register<global::Game.BaseConstructionSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();
            builder.Register<global::Game.OutpostsSystem>(Lifetime.Singleton).AsSelf().As<IGameSystem>();

            return;
        }

        public static IReadOnlyList<IGameSystem> ResolveSystems(IObjectResolver resolver)
        {
            var systems = new IGameSystem[]
            {
                resolver.Resolve<global::Game.BaseProductionSystem>(),
                resolver.Resolve<global::Game.CharacterSpawnSystem>(),
                resolver.Resolve<global::Game.CharacterCombatSystem>(),
                resolver.Resolve<global::Game.ConstructionsBehaviourSystem>(),
                resolver.Resolve<global::Game.CoreFreeRoamCameraSystem>(),
                resolver.Resolve<global::Game.BaseConstructionSystem>(),
                resolver.Resolve<global::Game.OutpostsSystem>(),
            };

            return systems;
        }
    }
}
