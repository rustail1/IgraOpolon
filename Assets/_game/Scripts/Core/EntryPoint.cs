using System.Threading;
using UnityEngine;
using Whaledevelop;
using Whaledevelop.Services;
using Whaledevelop.Systems;
using Whaledevelop.VContainer;

namespace Game
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class EntryPoint : EntryPointBase
    {
        private readonly IRuntimeServicesContainer _runtimeServicesContainer;
        private readonly IRuntimeModelsContainer _runtimeModelsContainer;
        private readonly IRuntimeSystemsContainer _runtimeSystemsContainer;
        private readonly IUpdatesDispatcher _updatesDispatcher;
        private readonly IUGUIService _uguiService;

        public EntryPoint(
            IRuntimeServicesContainer runtimeServicesContainer,
            IRuntimeModelsContainer runtimeModelsContainer,
            IRuntimeSystemsContainer runtimeSystemsContainer,
            IUpdatesDispatcher updatesDispatcher,
            IUGUIService uguiService)
        {
            _runtimeServicesContainer = runtimeServicesContainer;
            _runtimeModelsContainer = runtimeModelsContainer;
            _runtimeSystemsContainer = runtimeSystemsContainer;
            _updatesDispatcher = updatesDispatcher;
            _uguiService = uguiService;
        }

        public override async Awaitable StartAsync(CancellationToken cancellationToken = new())
        {
            foreach (var service in _runtimeServicesContainer.Services)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await service.InitializeAsync(cancellationToken);
            }

            foreach (var model in _runtimeModelsContainer.Models)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await model.InitializeAsync(cancellationToken);
            }

            foreach (var system in _runtimeSystemsContainer.Systems)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await system.InitializeAsync(cancellationToken);
                _updatesDispatcher.TryRegister(system);
            }

            if (!_uguiService.TryOpenView<CoreScreenUIViewModel, CoreScreenUIView>(
                    UIViewCode.Core,
                    new CoreScreenUIViewModel()))
            {
                Debug.LogError($"{nameof(CoreScreenUIView)} opening failed");
            }

            if (!_uguiService.TryOpenView<ConstructionsUIViewModel, ConstructionsUIView>(
                    UIViewCode.ConstructionsOverlay,
                    new ConstructionsUIViewModel()))
            {
                Debug.LogError($"{nameof(ConstructionsUIView)} opening failed");
            }

            if (!_uguiService.TryOpenView<PlayerSquadPanelUIViewModel, PlayerSquadPanelUIView>(
                    UIViewCode.PlayerSquadOverlay,
                    new PlayerSquadPanelUIViewModel()))
            {
                Debug.LogError($"{nameof(PlayerSquadPanelUIView)} opening failed");
            }

            if (!_uguiService.TryOpenView<OutpostsUIViewModel, OutpostsUIView>(
                    UIViewCode.OutpostsOverlay,
                    new OutpostsUIViewModel()))
            {
                Debug.LogError($"{nameof(OutpostsUIView)} opening failed");
            }
        }
    }
}
