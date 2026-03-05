using System;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class ColorSwatchButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image swatchImage;
        [SerializeField] private Image selectionRing;

        private ColorOption _option;
        private Action<ColorOption> _onSelected;

        /// <summary>Initializes the swatch with a color option and selection callback.</summary>
        public void Setup(ColorOption option, Action<ColorOption> onSelected)
        {
            _option = option;
            _onSelected = onSelected;

            swatchImage.color = option.color;
            button.onClick.AddListener(OnClicked);
            SetSelected(false);
        }

        /// <summary>Toggles the selection ring visibility.</summary>
        public void SetSelected(bool selected)
        {
            if (selectionRing != null)
                selectionRing.enabled = selected;
        }

        private void OnClicked() => _onSelected?.Invoke(_option);

        private void OnDestroy() => button.onClick.RemoveAllListeners();
    }
}
