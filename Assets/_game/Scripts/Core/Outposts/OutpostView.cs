using R3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    public sealed class OutpostView : MonoBehaviour
    {
        private readonly ReactiveProperty<OutpostTeam> _owner = new(OutpostTeam.None);
        private readonly ReactiveProperty<OutpostTeam> _capturingTeam = new(OutpostTeam.None);
        private readonly ReactiveProperty<float> _captureElapsedTime = new();
        private readonly ReactiveProperty<int> _captureCompletedRevision = new();

        [field: SerializeField]
        [field: BoxGroup("Capture")]
        public Transform CapturePoint { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public MeshRenderer ZoneRenderer { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public Material NeutralMaterial { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public Material PlayerAMaterial { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public Material PlayerBMaterial { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public Transform PlayerAFlag { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Visual")]
        public Transform PlayerBFlag { get; private set; }

        public ReadOnlyReactiveProperty<float> CaptureElapsedTime => _captureElapsedTime;

        public ReadOnlyReactiveProperty<OutpostTeam> CapturingTeam => _capturingTeam;

        public ReadOnlyReactiveProperty<int> CaptureCompletedRevision => _captureCompletedRevision;

        public float CaptureProgress { get; private set; }

        public OutpostTeam Owner => _owner.CurrentValue;

        public void SetState(
            OutpostTeam owner,
            OutpostTeam capturingTeam,
            float captureElapsedTime,
            float captureProgress)
        {
            _owner.Value = owner;
            _capturingTeam.Value = capturingTeam;
            _captureElapsedTime.Value = captureElapsedTime;
            CaptureProgress = captureProgress;
            ZoneRenderer.sharedMaterial = GetZoneMaterial(owner);
            PlayerAFlag.gameObject.SetActive(owner == OutpostTeam.Player);
            PlayerBFlag.gameObject.SetActive(owner == OutpostTeam.Enemy);
        }

        public void NotifyCaptureCompleted()
        {
            _captureCompletedRevision.Value++;
        }

        private Material GetZoneMaterial(OutpostTeam owner)
        {
            if (owner == OutpostTeam.Player)
            {
                return PlayerAMaterial;
            }

            if (owner == OutpostTeam.Enemy)
            {
                return PlayerBMaterial;
            }

            return NeutralMaterial;
        }
    }
}
