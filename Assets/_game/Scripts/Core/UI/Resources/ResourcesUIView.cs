using UnityEngine;
using UnityEngine.UI;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    // TODO rename to Currencies
    public sealed class ResourcesUIView : UIView<ResourcesUIViewModel>
    {
        [SerializeField]
        private Text _goldText;

        [SerializeField]
        private Text _followersText;

        private ResourcesUIViewModel _model;

        public override void Initialize(ResourcesUIViewModel model)
        {
            _model = model;
            _model.Changed += Refresh;
        }

        public override void Release()
        {
            _model.Changed -= Refresh;
            ((IReleasable)_model).Release();
            _model = null;
        }

        private void Refresh()
        {
            _goldText.text = _model.Gold.ToString();
            _followersText.text = _model.Followers.ToString();
        }
    }
}
