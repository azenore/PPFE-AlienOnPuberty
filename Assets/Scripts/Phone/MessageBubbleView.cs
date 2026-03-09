using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class MessageBubbleView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI senderNameText;
        [SerializeField] private Image bubbleBackground;

        [Header("Couleurs")]
        [SerializeField] private Color protagonistColor = new Color(0.33f, 0.64f, 1f);
        [SerializeField] private Color characterColor = new Color(0.92f, 0.92f, 0.92f);
        [SerializeField] private Color protagonistTextColor = Color.white;
        [SerializeField] private Color characterTextColor = Color.black;

        /// <summary>Configures the bubble with a message and the protagonist's display name.</summary>
        public void Setup(PhoneMessage message, string protagonistName)
        {
            messageText.text = message.text;

            HorizontalLayoutGroup hlg = GetComponent<HorizontalLayoutGroup>();

            if (message.IsFromProtagonist)
            {
                senderNameText.text = protagonistName;
                bubbleBackground.color = protagonistColor;
                messageText.color = protagonistTextColor;
                senderNameText.color = protagonistTextColor;
                hlg.childAlignment = TextAnchor.MiddleRight;
            }
            else
            {
                senderNameText.text = message.sender.characterName;
                bubbleBackground.color = characterColor;
                messageText.color = characterTextColor;
                senderNameText.color = characterTextColor;
                hlg.childAlignment = TextAnchor.MiddleLeft;
            }
        }
    }
}
