using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class DialogueBoxController : MonoBehaviour
    {
        private const string NamePlaceholder = "[NAME]";

        [SerializeField] private DialogueEngine engine;
        [SerializeField] private ProtagonistData protagonist;

        [Header("UI References")]
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private TextMeshProUGUI speakerNameText;
        [SerializeField] private TextMeshProUGUI dialogueText;

        [Tooltip("Portrait du protagoniste — toujours visible")]
        [SerializeField] private Image protagonistPortrait;

        private EmotionType _currentEmotion = EmotionType.Neutral;

        private void OnEnable()
        {
            engine.OnLineReady += ShowLine;
            engine.OnChoiceReady += OnChoiceReady;
            engine.OnProtagonistEmotionChanged += UpdateProtagonistEmotion;
        }

        private void OnDisable()
        {
            engine.OnLineReady -= ShowLine;
            engine.OnChoiceReady -= OnChoiceReady;
            engine.OnProtagonistEmotionChanged -= UpdateProtagonistEmotion;
        }

        private void Start()
        {
            RefreshProtagonistPortrait();
        }

        private void ShowLine(DialogueLine line)
        {
            dialogueBox.SetActive(true);

            if (line.IsNarrator)
            {
                speakerNameText.text = string.Empty;
            }
            else if (line.IsProtagonist)
            {
                speakerNameText.text = protagonist.playerName;
                speakerNameText.color = Color.white;
            }
            else
            {
                speakerNameText.text = line.speaker.characterName;
                speakerNameText.color = line.speaker.nameColor;
            }

            RefreshProtagonistPortrait();
            dialogueText.text = line.text.Replace(NamePlaceholder, protagonist.playerName);
        }

        private void OnChoiceReady(List<DialogueChoice> _)
        {
            dialogueBox.SetActive(true);
        }

        private void UpdateProtagonistEmotion(EmotionType emotion)
        {
            _currentEmotion = emotion;
            RefreshProtagonistPortrait();
        }

        /// <summary>Applique l'émotion courante sur le portrait du protagoniste.</summary>
        private void RefreshProtagonistPortrait()
        {
            if (protagonistPortrait == null) return;
            if (protagonist.characterData == null) return;

            protagonistPortrait.sprite = protagonist.characterData.GetSprite(_currentEmotion);
            protagonistPortrait.gameObject.SetActive(true);
        }
    }
}
