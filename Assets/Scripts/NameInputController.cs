using TMPro;
using UnityEngine;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class NameInputController : MonoBehaviour
    {
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private GameObject nameInputPanel;
        [SerializeField] private ChapterManager chapterManager;

        private const string DefaultName = "Yuki";

        private void Start()
        {
            nameInputField.text = protagonist.playerName;

            // Si une sauvegarde existe, le nom est déjà restauré → on skip le panneau
            bool skip = SaveSystem.HasSave();
            nameInputPanel.SetActive(!skip);
        }

        /// <summary>Called by ConfirmButton OnClick. Saves the name and starts the game.</summary>
        public void ConfirmName()
        {
            string input = nameInputField.text.Trim();
            protagonist.playerName = string.IsNullOrEmpty(input) ? DefaultName : input;

            nameInputPanel.SetActive(false);
            chapterManager.StartGame();
        }
    }
}
