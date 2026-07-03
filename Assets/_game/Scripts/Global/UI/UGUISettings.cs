using System.Collections.Generic;
using UnityEngine;
using Whaledevelop.UI;
using Whaledevelop.Utility;

namespace Game
{
    [CreateAssetMenu(fileName = "UGUISettings", menuName = "Game/UI/UGUISettings")]
    public class UGUISettings : ScriptableObject
    {
        [field: SerializeField]
        public UGUIRoot RootPrefab { get; private set; }

        [SerializeField]
        private SerializableDictionary<UIViewCode, UGUIViewSettings> _viewsSettings;

        public IReadOnlyDictionary<UIViewCode, UGUIViewSettings> ViewsSettings => _viewsSettings;
    }
}
