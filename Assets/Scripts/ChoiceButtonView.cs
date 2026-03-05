using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class ChoiceButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        /// <summary>Initializes the button with a choice and binds the engine callback.</summary>
        public void Setup(DialogueChoice choice, DialogueEngine engine)
        {
            label.text = choice.label;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => engine.SelectChoice(choice));
        }
    }
}
