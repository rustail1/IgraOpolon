using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    public sealed class ConstructionsUIViewModel : SyncLifetime, IUIViewModel
    {
        private readonly List<ConstructionSlotUIViewModel> _slots = new();
        private CoreStateSettings _stateSettings;
        private ICoreLevelsModel _levelsModel;
        private IConstructionsModel _constructionsModel;
        private ICurrenciesModel _resourcesModel;
        private Camera _camera;

        public IReadOnlyList<ConstructionSlotUIViewModel> Slots => _slots;

        [Inject]
        private void Construct(
            CoreStateSettings stateSettings,
            ICoreLevelsModel levelsModel,
            IConstructionsModel constructionsModel,
            ICurrenciesModel resourcesModel,
            Camera camera)
        {
            _stateSettings = stateSettings;
            _levelsModel = levelsModel;
            _constructionsModel = constructionsModel;
            _resourcesModel = resourcesModel;
            _camera = camera;
        }

        protected override void OnInitialize()
        {
            var baseView = _levelsModel.Level.CurrentValue.View.BaseConstructionView;

            foreach (var settings in _stateSettings.ConstructionSettings)
            {
                var slot = baseView.GetConstructionSlot(settings.ConstructionType);
                var slotModel = new ConstructionSlotUIViewModel(
                    settings,
                    slot,
                    _camera,
                    _constructionsModel,
                    _resourcesModel);
                slotModel.Initialize();
                _slots.Add(slotModel);
            }
        }

        protected override void OnRelease()
        {
            foreach (var slot in _slots)
            {
                slot.Release();
            }

            _slots.Clear();
        }
    }
}
