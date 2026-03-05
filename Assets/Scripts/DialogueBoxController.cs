using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class DialogueBoxController : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;

        [Header("UI References")]
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image characterPortrait;

        private void OnEnable()
        {
            engine.OnLineReady += ShowLine;
            engine.OnChoiceReady += _ => dialogueBox.SetActive(false);
        }

        private void OnDisable()
        {
            engine.OnLineReady -= ShowLine;
            engine.OnChoiceReady -= _ => dialogueBox.SetActive(false);
        }

        private void ShowLine(DialogueLine line)
        {
            dialogueBox.SetActive(true);

            if (line.IsNarrator)
            {
                speakerNameText.text = string.Empty;
                characterPortrait.gameObject.SetActive(false);
            }
            else
            {
                speakerNameText.text = line.speaker.characterName;
                speakerNameText.color = line.speaker.nameColor;
                characterPortrait.sprite = line.speaker.GetSprite(line.speakerEmotion);
                characterPortrait.gameObject.SetActive(characterPortrait.sprite != null);
            }

            dialogueText.text = line.text;
        }
    }
}
