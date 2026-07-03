using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Splines;

namespace Game
{
    public sealed class LineView : MonoBehaviour
    {
        // [field: SerializeField]
        // [field: BoxGroup("Splines")]
        // public SplineContainer FullSpline { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Spawn Points")]
        public Transform SpawnPointA { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Spawn Points")]
        public Transform SpawnPointB { get; private set; }

        [field: SerializeField]
        [field: BoxGroup("Outposts")]
        public OutpostView[] Outposts { get; private set; }

    }
}
