using TMPro;
using UnityEngine;
using VN.Data;

namespace VN.UI
{
    public class NameInputController : MonoBehaviour
    {
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private TMP_InputField nameInputField;
        [SerializeField] private GameObject nameInputScreen;
        [SerializeField] private GameObject gameCanvas;

        private const string DefaultName = "Yuki";

        private void Start()
        {
            nameInputScreen.SetActive(true);
            gameCanvas.SetActive(false);
            nameInputField.text = protagonist.playerName;
        }

        /// <summary>Called by the confirm button's OnClick event.</summary>
        public void ConfirmName()
        {
            string input = nameInputField.text.Trim();
            protagonist.playerName = string.IsNullOrEmpty(input) ? DefaultName : input;

            nameInputScreen.SetActive(false);
            gameCanvas.SetActive(true);
        }
    }
}
