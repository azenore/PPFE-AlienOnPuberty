using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class CharacterCustomizationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private GameSaveController gameSaveController;

        [Header("Panels")]
        [SerializeField] private GameObject customizationPanel;

        [Header("Menu Buttons")]
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueButton;

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

        private void Start()
        {
            nameInputField.text = protagonist.playerName;

            BuildSwatches(hairColorOptions, hairSwatchContainer, _hairSwatches, OnHairSelected);
            BuildSwatches(eyeColorOptions, eyeSwatchContainer, _eyeSwatches, OnEyeSelected);

            if (hairColorOptions.Count > 0) OnHairSelected(hairColorOptions[0]);
            if (eyeColorOptions.Count > 0) OnEyeSelected(eyeColorOptions[0]);

            // Affiche le bouton Continuer uniquement si une sauvegarde existe
            bool hasSave = SaveSystem.HasSave();
            continueButton.gameObject.SetActive(hasSave);

            customizationPanel.SetActive(true);
        }

        /// <summary>Appelé par le bouton Nouvelle Partie.</summary>
        public void Confirm()
        {
            string trimmedName = nameInputField.text.Trim();
            protagonist.playerName = string.IsNullOrEmpty(trimmedName) ? "Yuki" : trimmedName;

            if (_selectedHairColor != null) protagonist.hairColor = _selectedHairColor.color;
            if (_selectedEyeColor != null) protagonist.eyeColor = _selectedEyeColor.color;

            SaveSystem.DeleteSave();
            customizationPanel.SetActive(false);
            chapterManager.StartGame();
        }

        /// <summary>Appelé par le bouton Continuer — charge la dernière sauvegarde.</summary>
        public void Continue()
        {
            customizationPanel.SetActive(false);
            gameSaveController.LoadGame();
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
