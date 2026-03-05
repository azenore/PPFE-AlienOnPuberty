using UnityEngine;
using UnityEngine.UI;
using VN.Runtime;

namespace VN.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject customizationPanel;
        [SerializeField] private GameObject optionsPanel;
        [SerializeField] private GameObject gamePanel;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button continueButton;

        [Header("References")]
        [SerializeField] private GameSaveController gameSaveController;
        [SerializeField] private CharacterCustomizationController customizationController;

        private void Start()
        {
            ShowMainMenu();
        }

        /// <summary>Shows the main menu and hides all other panels.</summary>
        public void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            customizationPanel.SetActive(false);
            optionsPanel.SetActive(false);
            gamePanel.SetActive(false);

            // Le bouton Continuer n'apparaît que si une sauvegarde existe
            continueButton.gameObject.SetActive(SaveSystem.HasSave());
        }

        /// <summary>Called by NewGameButton OnClick.</summary>
        public void OnNewGame()
        {
            mainMenuPanel.SetActive(false);
            customizationPanel.SetActive(true);
            customizationController.PrepareCustomization();
        }

        /// <summary>Called by ContinueButton OnClick.</summary>
        public void OnContinue()
        {
            mainMenuPanel.SetActive(false);
            gamePanel.SetActive(true);
            gameSaveController.LoadGame();
        }

        /// <summary>Called by OptionsButton OnClick.</summary>
        public void OnOptions()
        {
            mainMenuPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }

        /// <summary>Called by CloseButton in OptionsPanel OnClick.</summary>
        public void OnCloseOptions()
        {
            optionsPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }

        /// <summary>Called by QuitButton OnClick.</summary>
        public void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>Called by CharacterCustomizationController when the player confirms and starts.</summary>
        public void OnGameStarted()
        {
            customizationPanel.SetActive(false);
            gamePanel.SetActive(true);
        }
    }
}
