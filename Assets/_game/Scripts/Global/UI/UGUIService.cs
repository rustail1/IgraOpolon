using UnityEngine;
using Whaledevelop.Services;
using Whaledevelop.UI;

namespace Game
{
    [GameService]
    public class UGUIService : UIService<UIViewCode>, IUGUIService
    {
        private readonly UGUISettings _settings;

        private UGUIRoot _root;

        public UGUIService(UGUISettings settings)
        {
            _settings = settings;
        }

        protected override void OnInitialize()
        {
            _root = Object.Instantiate(_settings.RootPrefab);
            base.OnInitialize();
        }

        protected override TView CreateView<TView>(UIViewCode code)
        {
            if (!_settings.ViewsSettings.TryGetValue(code, out var settings))
            {
                return default;
            }
            var parent = _root.GetLayer(settings.LayerCode);
            var view = Object.Instantiate(settings.ViewPrefab, parent);

            if (view is TView typedView)
            {
                return typedView;
            }

            Object.Destroy(view.gameObject);

            return default;
        }

        protected override void DestroyView(UIViewCode code, IUIView view)
        {
            if (view is not UIView unityView)
            {
                return;
            }

            Object.Destroy(unityView.gameObject);
        }
    }
}
