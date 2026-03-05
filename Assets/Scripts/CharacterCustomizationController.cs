using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private MainMenuController mainMenuController;

        [Header("Name")]
        [SerializeField] private TMP_InputField nameInputField;

        [Header("Hair Colors")]
        [SerializeField] private List<ColorOption> hairColorOptions;
        [SerializeField] private Transform hairSwatchContainer;
        [SerializeField] private ColorSwatchButton colorSwatchPrefab;

        [Header("Eye Colors")]
        [SerializeField] private List<ColorOption> eyeColorOptions;
        [SerializeField] private Transform eyeSwatchContainer;

        private ColorOption _selectedHairColor;
        private ColorOption _selectedEyeColor;

        private readonly List<ColorSwatchButton> _hairSwatches = new();
        private readonly List<ColorSwatchButton> _eyeSwatches = new();

        private bool _swatchesBuilt = false;

        /// <summary>Called by MainMenuController when entering customization screen.</summary>
        public void PrepareCustomization()
        {
            nameInputField.text = protagonist.playerName;

            if (!_swatchesBuilt)
            {
                BuildSwatches(hairColorOptions, hairSwatchContainer, _hairSwatches, OnHairSelected);
                BuildSwatches(eyeColorOptions, eyeSwatchContainer, _eyeSwatches, OnEyeSelected);
                _swatchesBuilt = true;
            }

            if (hairColorOptions.Count > 0) OnHairSelected(hairColorOptions[0]);
            if (eyeColorOptions.Count > 0) OnEyeSelected(eyeColorOptions[0]);
        }

        /// <summary>Called by StartButton OnClick. Applies customization and launches the game.</summary>
        public void Confirm()
        {
            string trimmedName = nameInputField.text.Trim();
            protagonist.playerName = string.IsNullOrEmpty(trimmedName) ? "Yuki" : trimmedName;

            if (_selectedHairColor != null) protagonist.hairColor = _selectedHairColor.color;
            if (_selectedEyeColor != null) protagonist.eyeColor = _selectedEyeColor.color;

            SaveSystem.DeleteSave();
            mainMenuController.OnGameStarted();
            chapterManager.StartGame();
        }

        private void BuildSwatches(
            List<ColorOption> options,
            Transform container,
            List<ColorSwatchButton> swatchList,
            System.Action<ColorOption> callback)
        {
            foreach (var option in options)
            {
                var swatch = Instantiate(colorSwatchPrefab, container);
                swatch.Setup(option, callback);
                swatchList.Add(swatch);
            }
        }

        private void OnHairSelected(ColorOption option)
        {
            _selectedHairColor = option;
            UpdateSelection(_hairSwatches, hairColorOptions, option);
        }

        private void OnEyeSelected(ColorOption option)
        {
            _selectedEyeColor = option;
            UpdateSelection(_eyeSwatches, eyeColorOptions, option);
        }

        private void UpdateSelection(
            List<ColorSwatchButton> swatches,
            List<ColorOption> options,
            ColorOption selected)
        {
            for (int i = 0; i < swatches.Count; i++)
                swatches[i].SetSelected(options[i] == selected);
        }
    }
}
