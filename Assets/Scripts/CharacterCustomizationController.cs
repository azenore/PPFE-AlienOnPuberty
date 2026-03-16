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
        [SerializeField] private GameSaveController gameSaveController;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private MainMenuController mainMenuController;

        [Header("Name")]
        [SerializeField] private TMP_InputField nameInputField;

        [Header("Hair Color")]
        [SerializeField] private List<ColorOption> hairColorOptions;
        [SerializeField] private ArrowSelector hairColorSelector;

        [Header("Eye Color")]
        [SerializeField] private List<ColorOption> eyeColorOptions;
        [SerializeField] private ArrowSelector eyeColorSelector;

        [Header("Preview")]
        [SerializeField] private Image protagonistPreviewImage;

        private ColorOption _selectedHairColor;
        private ColorOption _selectedEyeColor;

        /// <summary>Called by MainMenuController when entering the customization screen.</summary>
        public void PrepareCustomization()
        {
            nameInputField.text = protagonist.playerName;

            hairColorSelector.Setup(hairColorOptions, option =>
            {
                _selectedHairColor = option;
                protagonist.hairId = option.id;
                RefreshPreviewSprite();
            });

            eyeColorSelector.Setup(eyeColorOptions, option =>
            {
                _selectedEyeColor = option;
                protagonist.eyeId = option.id;
                RefreshPreviewSprite();
            });
        }

        /// <summary>Called by the Confirm button OnClick. Applies customization and launches the game.</summary>
        public void Confirm()
        {
            string trimmedName = nameInputField.text.Trim();
            protagonist.playerName = string.IsNullOrEmpty(trimmedName) ? "Yuki" : trimmedName;

            if (_selectedHairColor != null) protagonist.hairColor = _selectedHairColor.color;
            if (_selectedEyeColor != null) protagonist.eyeColor = _selectedEyeColor.color;

            gameSaveController.ApplyCustomization();

            SaveSystem.DeleteSave();
            mainMenuController.OnGameStarted();
            chapterManager.StartGame();
        }

        private void RefreshPreviewSprite()
        {
            if (protagonistPreviewImage == null) return;

            Sprite sprite = protagonist.GetSprite(EmotionType.Neutral);
            protagonistPreviewImage.sprite = sprite;
            protagonistPreviewImage.gameObject.SetActive(sprite != null);
        }
    }
}
