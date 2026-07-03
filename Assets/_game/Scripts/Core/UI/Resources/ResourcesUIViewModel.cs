using System;
using R3;
using VContainer;
using Whaledevelop;
using Whaledevelop.UI;

namespace Game
{
    public sealed class ResourcesUIViewModel : SyncLifetime, IUIViewModel
    {
        private ICurrenciesModel _resourcesModel;
        private IDisposable _goldSubscription;
        private IDisposable _followersSubscription;

        public event Action Changed;

        public int Gold => _resourcesModel.Gold.CurrentValue;

        public int Followers => _resourcesModel.Followers.CurrentValue;

        [Inject]
        private void Construct(ICurrenciesModel resourcesModel)
        {
            _resourcesModel = resourcesModel;
        }

        protected override void OnInitialize()
        {
            _goldSubscription = _resourcesModel.Gold.Subscribe(_ => Changed?.Invoke());
            _followersSubscription = _resourcesModel.Followers.Subscribe(_ => Changed?.Invoke());
        }

        protected override void OnRelease()
        {
            _goldSubscription.Dispose();
            _followersSubscription.Dispose();
        }
    }
}
