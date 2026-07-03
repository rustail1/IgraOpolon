using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.Utility;

namespace Game
{
    public class UGUIRoot : MonoBehaviour
    {
        [SerializeField]
        private SerializableDictionary<UILayerCode, RectTransform> _layers;

        public Transform GetLayer(UILayerCode layerCode)
        {
            return _layers.TryGetValue(layerCode, out var rect) ? rect : _layers[UILayerCode.Screen];
        }
    }
}
