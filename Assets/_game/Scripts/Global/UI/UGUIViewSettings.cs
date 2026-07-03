using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Whaledevelop.UI;

namespace Game
{
    [Serializable]
    public class UGUIViewSettings
    {
        [field: FoldoutGroup("ViewSettings")]
        [field: SerializeField]
        public UIView ViewPrefab { get; private set; }

        [field: FoldoutGroup("ViewSettings")]
        [field: Tooltip(
            "Hud, sorting 0: persistent gameplay UI.\n" +
            "Screen, sorting 100: fullscreen screens.\n" +
            "Popup, sorting 200: popups, dialogs.\n" +
            "Overlay, sorting 300: tutorials, dimming, global blockers.")]
        [field: SerializeField]
        public UILayerCode LayerCode { get; private set; }

        [field: FoldoutGroup("Debug")]
        [field: SerializeReference]
        public IUIViewModel TestModel { get; private set; }
    }
}
