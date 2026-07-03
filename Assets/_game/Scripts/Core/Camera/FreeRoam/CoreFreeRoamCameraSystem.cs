using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using Whaledevelop;
using Whaledevelop.Systems;

namespace Game
{
    [GameSystem(nameof(GameStateCode.Core))]
    public sealed class CoreFreeRoamCameraSystem : SyncLifetime, IGameSystem, IUpdate
    {
        private readonly CoreStateSettings _stateSettings;
        private readonly ICoreLevelsModel _levelsModel;

        private FreeRoamCameraView _cameraView;
        private InputAction _zoomAction;
        private InputAction _rotationAction;
        private InputAction _movementAction;

        [Inject]
        public CoreFreeRoamCameraSystem(
            CoreStateSettings stateSettings,
            ICoreLevelsModel levelsModel)
        {
            _stateSettings = stateSettings;
            _levelsModel = levelsModel;
        }

        protected override void OnInitialize()
        {
            _cameraView = Object.Instantiate(_stateSettings.FreeRoamCameraPrefab);
            _cameraView.ApplyStartPose(_levelsModel.Level.CurrentValue.View.StartCameraPose);

            _zoomAction = new InputAction("FreeRoamZoom", InputActionType.Value, "<Mouse>/scroll/y");
            _zoomAction.Enable();

            _rotationAction = new InputAction("FreeRoamRotate", InputActionType.Value);
            _rotationAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/e")
                .With("Positive", "<Keyboard>/q");
            _rotationAction.Enable();

            _movementAction = new InputAction("FreeRoamMove", InputActionType.Value);
            _movementAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            _movementAction.Enable();
        }

        protected override void OnRelease()
        {
            _zoomAction.Disable();
            _zoomAction.Dispose();
            _zoomAction = null;

            _rotationAction.Disable();
            _rotationAction.Dispose();
            _rotationAction = null;

            _movementAction.Disable();
            _movementAction.Dispose();
            _movementAction = null;

            if (_cameraView)
            {
                Object.Destroy(_cameraView.gameObject);
                _cameraView = null;
            }
        }

        public void OnUpdate()
        {
            _cameraView.UpdateCamera(
                _movementAction.ReadValue<Vector2>(),
                _rotationAction.ReadValue<float>(),
                _zoomAction.ReadValue<float>(),
                Time.deltaTime);

            return;
        }
    }
}
