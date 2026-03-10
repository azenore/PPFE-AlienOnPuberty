using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class PhoneChoiceButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI label;

        /// <summary>Initializes the button with a phone choice.</summary>
        public void Setup(PhoneChoice choice, PhoneEngine engine, AffinitySystem affinitySystem)
        {
            label.text = choice.label;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => engine.SelectChoice(choice, affinitySystem));
        }
    }
}
