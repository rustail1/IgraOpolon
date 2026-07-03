using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whaledevelop.Services;
using Whaledevelop.Systems;
using Whaledevelop.VContainer;

namespace Game
{
    public sealed class CoreLifetimeScope : GameLifetimeScopeBase<EntryPoint>
    {
        [SerializeField]
        private CoreStateSettings _stateSettings;

        [SerializeField]
        private Camera _camera;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            builder.RegisterInstance(_stateSettings);
            builder.RegisterInstance(_stateSettings.BaseSettings);
            builder.RegisterInstance(_stateSettings.UGUISettings);
            builder.RegisterInstance(_camera);
            RegisterServices(builder);
            RegisterModels(builder);
            RegisterSystems(builder);

            return;
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            RuntimeServicesDebugContainer.Clear();
            RuntimeModelsDebugContainer.Clear();
            RuntimeSystemsDebugContainer.Clear();
#endif
            base.OnDestroy();
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            var runtimeServicesContainer = new RuntimeServicesContainer();
            builder.RegisterInstance(runtimeServicesContainer)
                .As<RuntimeServicesContainer>()
                .As<IRuntimeServicesContainer>();

            GameServicesComposition.RegisterServices(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                var runtimeServices = GameServicesComposition.ResolveServices(resolver);
                runtimeServicesContainer.SetServices(runtimeServices);

#if UNITY_EDITOR
                RuntimeServicesDebugContainer.SetServices(runtimeServices);
#endif
            });

            return;
        }

        private static void RegisterModels(IContainerBuilder builder)
        {
            var runtimeModelsContainer = new RuntimeModelsContainer();
            builder.RegisterInstance(runtimeModelsContainer)
                .As<RuntimeModelsContainer>()
                .As<IRuntimeModelsContainer>();
            builder.Register<SharedGameModelsContainer>(Lifetime.Singleton)
                .As<ISharedGameModelsContainer>();

            GameRuntimeDataComposition.RegisterModels(builder);
            CoreRuntimeDataComposition.RegisterModels(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                var gameModels = GameRuntimeDataComposition.ResolveModels(resolver);
                var coreModels = CoreRuntimeDataComposition.ResolveModels(resolver);
                var runtimeModels = Combine(gameModels, coreModels);
                runtimeModelsContainer.SetModels(runtimeModels);

#if UNITY_EDITOR
                RuntimeModelsDebugContainer.SetModels(runtimeModels);
#endif
            });

            return;
        }

        private static void RegisterSystems(IContainerBuilder builder)
        {
            var runtimeSystemsContainer = new RuntimeSystemsContainer();
            builder.RegisterInstance(runtimeSystemsContainer)
                .As<RuntimeSystemsContainer>()
                .As<IRuntimeSystemsContainer>();

            GameRuntimeDataComposition.RegisterSystems(builder);
            CoreRuntimeDataComposition.RegisterSystems(builder);

            builder.RegisterBuildCallback(resolver =>
            {
                var gameSystems = GameRuntimeDataComposition.ResolveSystems(resolver);
                var coreSystems = CoreRuntimeDataComposition.ResolveSystems(resolver);
                var runtimeSystems = Combine(gameSystems, coreSystems);
                runtimeSystemsContainer.SetSystems(runtimeSystems);

#if UNITY_EDITOR
                RuntimeSystemsDebugContainer.SetSystems(runtimeSystems);
#endif
            });

            return;
        }

        private static IReadOnlyList<T> Combine<T>(
            IReadOnlyList<T> firstItems,
            IReadOnlyList<T> secondItems)
        {
            var items = new T[firstItems.Count + secondItems.Count];

            for (var index = 0; index < firstItems.Count; index++)
            {
                items[index] = firstItems[index];
            }

            for (var index = 0; index < secondItems.Count; index++)
            {
                items[firstItems.Count + index] = secondItems[index];
            }

            return items;
        }
    }
}
