using UnityEngine;
using Whaledevelop.Utility;

namespace Game
{
    public class LevelView : MonoBehaviour
    {
        [field: SerializeField]
        public BaseConstructionView BaseConstructionView { get; private set; }

        [field: SerializeField]
        public BaseConstructionView EnemyBaseConstructionView { get; private set; }

        [field: SerializeField]
        public Transform StartCameraPose { get; private set; }

        [field: SerializeField]
        public Transform CharactersRoot { get; private set; }

        [SerializeField]
        private SerializableDictionary<LineCode, LineView> _lines;

        public LineView GetLine(LineCode lineCode) => _lines[lineCode];

    }
}
