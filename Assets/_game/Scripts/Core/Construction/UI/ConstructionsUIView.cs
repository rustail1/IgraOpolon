using System.Collections.Generic;
using UnityEngine;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    public sealed class ConstructionsUIView : UIView<ConstructionsUIViewModel>
    {
        [SerializeField]
        private RectTransform _slotsRoot;

        [SerializeField]
        private ConstructionSlotUIView _slotPrefab;

        private readonly List<ConstructionSlotUIView> _slotViews = new();
        private ConstructionsUIViewModel _model;

        public override void Initialize(ConstructionsUIViewModel model)
        {
            _model = model;

            foreach (var slotModel in _model.Slots)
            {
                var slotView = Instantiate(_slotPrefab, _slotsRoot);
                slotView.Initialize(slotModel);
                _slotViews.Add(slotView);
            }
        }

        private void LateUpdate()
        {
            foreach (var slotView in _slotViews)
            {
                slotView.UpdateScreenPosition();
            }
        }

        public override void Release()
        {
            foreach (var slotView in _slotViews)
            {
                slotView.Release();
                Destroy(slotView.gameObject);
            }

            _slotViews.Clear();
            ((IReleasable)_model).Release();
            _model = null;
        }
    }
}
