using Whaledevelop.UI;
using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Whaledevelop;

namespace Game
{
    public sealed class OutpostsUIView : UIView<OutpostsUIViewModel>
    {
        [SerializeField]
        private Image _captureProgressImage;

        [SerializeField]
        private float _captureIndicatorHideDelay = 2f;

        private readonly List<Image> _captureProgressViews = new();
        private readonly List<IDisposable> _subscriptions = new();
        private readonly Dictionary<Image, float> _captureIndicatorHideTimes = new();
        private readonly List<Image> _expiredCaptureProgressViews = new();
        private OutpostsUIViewModel _model;

        private void Awake()
        {
            _captureProgressImage.gameObject.SetActive(false);
        }

        public override void Initialize(OutpostsUIViewModel model)
        {
            _model = model;
            ((IInitializable)_model).Initialize();

            foreach (var outpostView in _model.Outposts)
            {
                var captureProgressView = Instantiate(_captureProgressImage, transform);
                captureProgressView.fillAmount = 0f;
                captureProgressView.gameObject.SetActive(false);
                _captureProgressViews.Add(captureProgressView);
                _subscriptions.Add(outpostView.CaptureElapsedTime.Subscribe(_ =>
                    UpdateCaptureProgress(captureProgressView, outpostView)));
                _subscriptions.Add(outpostView.CaptureCompletedRevision.Subscribe(revision =>
                    ShowCompletedCapture(captureProgressView, outpostView, revision)));
            }
        }

        private void LateUpdate()
        {
            HideCompletedCaptureIndicators();

            for (var i = 0; i < _captureProgressViews.Count; i++)
            {
                _captureProgressViews[i].transform.position = _model.Camera.WorldToScreenPoint(
                    _model.Outposts[i].CapturePoint.position);
            }
        }

        public override void Release()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }

            foreach (var captureProgressView in _captureProgressViews)
            {
                Destroy(captureProgressView.gameObject);
            }

            _subscriptions.Clear();
            _captureProgressViews.Clear();
            _captureIndicatorHideTimes.Clear();
            _expiredCaptureProgressViews.Clear();
            ((IReleasable)_model).Release();
            _model = null;
        }

        private void UpdateCaptureProgress(Image captureProgressView, OutpostView outpostView)
        {
            var progress = outpostView.CaptureProgress;
            if (_captureIndicatorHideTimes.ContainsKey(captureProgressView) && progress > 0f)
            {
                _captureIndicatorHideTimes.Remove(captureProgressView);
            }

            if (_captureIndicatorHideTimes.ContainsKey(captureProgressView))
            {
                return;
            }

            captureProgressView.fillAmount = progress;
            captureProgressView.color = _model.GetTeamColor(outpostView.CapturingTeam.CurrentValue);
            captureProgressView.gameObject.SetActive(progress > 0f);
        }

        private void ShowCompletedCapture(
            Image captureProgressView,
            OutpostView outpostView,
            int revision)
        {
            if (revision == 0)
            {
                return;
            }

            captureProgressView.fillAmount = 1f;
            captureProgressView.color = _model.GetTeamColor(outpostView.Owner);
            captureProgressView.gameObject.SetActive(true);
            _captureIndicatorHideTimes[captureProgressView] = Time.unscaledTime + _captureIndicatorHideDelay;
        }

        private void HideCompletedCaptureIndicators()
        {
            foreach (var captureIndicatorHideTime in _captureIndicatorHideTimes)
            {
                if (Time.unscaledTime < captureIndicatorHideTime.Value)
                {
                    continue;
                }

                captureIndicatorHideTime.Key.fillAmount = 0f;
                captureIndicatorHideTime.Key.gameObject.SetActive(false);
                _expiredCaptureProgressViews.Add(captureIndicatorHideTime.Key);
            }

            foreach (var captureProgressView in _expiredCaptureProgressViews)
            {
                _captureIndicatorHideTimes.Remove(captureProgressView);
            }

            _expiredCaptureProgressViews.Clear();
        }
    }
}
