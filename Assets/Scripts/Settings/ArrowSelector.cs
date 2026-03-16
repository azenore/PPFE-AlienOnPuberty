using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    /// <summary>Generic left/right arrow selector cycling through a list of ColorOptions.</summary>
    public class ArrowSelector : MonoBehaviour
    {
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Image colorPreview;
        [SerializeField] private TMP_Text optionLabel;

        private List<ColorOption> _options;
        private int _currentIndex;
        private Action<ColorOption> _onChanged;

        /// <summary>Initializes the selector with a list of options and a change callback.</summary>
        public void Setup(List<ColorOption> options, Action<ColorOption> onChanged)
        {
            _options = options;
            _onChanged = onChanged;
            _currentIndex = 0;

            previousButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            previousButton.onClick.AddListener(Previous);
            nextButton.onClick.AddListener(Next);

            Refresh();
        }

        /// <summary>Returns the currently selected ColorOption.</summary>
        public ColorOption Current => (_options != null && _options.Count > 0)
            ? _options[_currentIndex]
            : null;

        private void Previous()
        {
            _currentIndex = (_currentIndex - 1 + _options.Count) % _options.Count;
            Refresh();
        }

        private void Next()
        {
            _currentIndex = (_currentIndex + 1) % _options.Count;
            Refresh();
        }

        private void Refresh()
        {
            if (_options == null || _options.Count == 0) return;

            var option = _options[_currentIndex];
            if (colorPreview != null) colorPreview.color = option.color;
            if (optionLabel != null) optionLabel.text = option.optionName;

            _onChanged?.Invoke(option);
        }

        private void OnDestroy()
        {
            previousButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
        }
    }
}
