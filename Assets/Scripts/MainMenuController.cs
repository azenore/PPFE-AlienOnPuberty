using UnityEngine;
using UnityEngine.UI;
using VN.Data;
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
        [SerializeField] private GameObject affinityBarPanel;
        [SerializeField] private GameObject confirmationPanel;

        [Header("Main Menu Buttons")]
        [SerializeField] private Button continueButton;

        [Header("References")]
        [SerializeField] private GameSaveController gameSaveController;
        [SerializeField] private CharacterCustomizationController customizationController;
        [SerializeField] private AffinityBarView affinityBarView;
        [SerializeField] private ProtagonistData protagonist;

        private void Start()
        {
            gameSaveController.OnSaved += RefreshContinueButton;
            ShowMainMenu();
        }

        private void OnDestroy()
        {
            gameSaveController.OnSaved -= RefreshContinueButton;
        }

        /// <summary>Shows the main menu and hides all other panels.</summary>
        public void ShowMainMenu()
        {
            mainMenuPanel.SetActive(true);
            customizationPanel.SetActive(false);
            optionsPanel.SetActive(false);
            gamePanel.SetActive(false);
            affinityBarPanel.SetActive(false);
            confirmationPanel.SetActive(false);
            RefreshContinueButton();
        }

        /// <summary>Called by the Home button OnClick. Saves progression then returns to main menu.</summary>
        public void OnHome()
        {
            gameSaveController.SaveGame();
            ShowMainMenu();
        }

        /// <summary>Refreshes the continue button visibility based on save file existence.</summary>
        private void RefreshContinueButton()
        {
            continueButton.gameObject.SetActive(SaveSystem.HasSave());
        }

        /// <summary>Called by NewGameButton OnClick.</summary>
        public void OnNewGame()
        {
            if (gameSaveController.HasSave())
            {
                confirmationPanel.SetActive(true);
            }
            else
            {
                StartNewGame();
            }
        }

        /// <summary>Called by the ConfirmationPanel "Oui" button OnClick.</summary>
        public void OnConfirmNewGame()
        {
            confirmationPanel.SetActive(false);
            StartNewGame();
        }

        private void StartNewGame()
        {
            SaveSystem.DeleteSave();
            protagonist.ResetAffinities();
            mainMenuPanel.SetActive(false);
            customizationPanel.SetActive(true);
            customizationController.PrepareCustomization();
        }

        /// <summary>Called by ContinueButton OnClick.</summary>
        public void OnContinue()
        {
            mainMenuPanel.SetActive(false);
            gamePanel.SetActive(true);
            affinityBarPanel.SetActive(true);
            gameSaveController.LoadGame();
            affinityBarView.ForceRefresh();
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
