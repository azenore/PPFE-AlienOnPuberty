using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class PhoneChatController : MonoBehaviour
    {
        private const float BubbleAnimDuration = 0.18f;

        [Header("References")]
        [SerializeField] private PhoneEngine phoneEngine;
        [SerializeField] private ProtagonistData protagonist;

        [Header("UI")]
        [SerializeField] private GameObject phonePanel;
        [SerializeField] private Transform messagesContainer;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private MessageBubbleView bubblePrefab;
        [SerializeField] private Button advanceButton;

        [Tooltip("Texte affichant le nom du groupe ou les participants.")]
        [SerializeField] private TextMeshProUGUI participantsText;

        private readonly List<MessageBubbleView> _bubbles = new();

        private void OnEnable()
        {
            phoneEngine.OnMessageReady += HandleMessageReady;
            phoneEngine.OnChoiceReady += HandleChoiceReady;
            phoneEngine.OnConversationFinished += HandleConversationFinished;
            phoneEngine.OnPhoneChapterFinished += HandlePhoneChapterFinished;
        }

        private void OnDisable()
        {
            phoneEngine.OnMessageReady -= HandleMessageReady;
            phoneEngine.OnChoiceReady -= HandleChoiceReady;
            phoneEngine.OnConversationFinished -= HandleConversationFinished;
            phoneEngine.OnPhoneChapterFinished -= HandlePhoneChapterFinished;
        }

        /// <summary>Shows the phone panel, clears previous messages and updates the header.</summary>
        public void OpenChat(PhoneChapter chapter)
        {
            ClearBubbles();
            if (participantsText != null)
                participantsText.text = chapter.GetHeaderLabel();
            advanceButton.gameObject.SetActive(true);
            phonePanel.SetActive(true);
        }

        /// <summary>
        /// Shows the phone panel and instantly replays all messages up to upToIndex inclusive.
        /// Used when restoring a save so the player sees the full conversation history.
        /// </summary>
        public void OpenChatWithReplay(PhoneChapter chapter, int upToIndex)
        {
            ClearBubbles();

            if (participantsText != null)
                participantsText.text = chapter.GetHeaderLabel();

            int clampedIndex = Mathf.Clamp(upToIndex, 0, chapter.messages.Count - 1);

            for (int i = 0; i <= clampedIndex; i++)
            {
                MessageBubbleView bubble = Instantiate(bubblePrefab, messagesContainer);
                bubble.SetupInstant(chapter.messages[i], protagonist.playerName);
                _bubbles.Add(bubble);
            }

            advanceButton.gameObject.SetActive(true);
            phonePanel.SetActive(true);

            StartCoroutine(ScrollToBottomImmediate());
        }

        /// <summary>Hides the phone panel.</summary>
        public void CloseChat()
        {
            phonePanel.SetActive(false);
        }

        private void HandleMessageReady(PhoneMessage message)
        {
            advanceButton.gameObject.SetActive(true);

            MessageBubbleView bubble = Instantiate(bubblePrefab, messagesContainer);
            bubble.Setup(message, protagonist.playerName);
            _bubbles.Add(bubble);

            StartCoroutine(ScrollToBottom());
        }

        private void HandleChoiceReady(List<PhoneChoice> _)
        {
            advanceButton.gameObject.SetActive(false);
        }

        // Scroll après animation pour les nouveaux messages
        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForSeconds(BubbleAnimDuration);
            yield return null;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 0f;
        }

        // Scroll immédiat après replay (pas d'animation donc 2 frames suffisent)
        private IEnumerator ScrollToBottomImmediate()
        {
            yield return null;
            yield return null;

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void HandleConversationFinished(DialogueChapter next)
        {
            CloseChat();
        }

        private void HandlePhoneChapterFinished(PhoneChapter next)
        {
            ClearBubbles();
            phoneEngine.LoadPhoneChapter(next);
        }

        private void ClearBubbles()
        {
            foreach (var bubble in _bubbles)
                Destroy(bubble.gameObject);
            _bubbles.Clear();
        }

        /// <summary>
        /// Shows the phone panel, replays all messages up to choiceMessageIndex inclusive,
        /// then appends the chosen option as a protagonist bubble.
        /// Used when restoring a save that ended on a choice.
        /// </summary>
        public void OpenChatWithReplay(PhoneChapter chapter, int choiceMessageIndex, int choiceIndex)
        {
            ClearBubbles();

            if (participantsText != null)
                participantsText.text = chapter.GetHeaderLabel();

            int clampedIndex = Mathf.Clamp(choiceMessageIndex, 0, chapter.messages.Count - 1);

            for (int i = 0; i <= clampedIndex; i++)
            {
                MessageBubbleView bubble = Instantiate(bubblePrefab, messagesContainer);
                bubble.SetupInstant(chapter.messages[i], protagonist.playerName);
                _bubbles.Add(bubble);
            }

            // Affiche le choix sélectionné comme bulle protagoniste
            List<PhoneChoice> choices = chapter.messages[clampedIndex].choices;
            if (choiceIndex >= 0 && choiceIndex < choices.Count)
            {
                PhoneMessage choiceBubble = new PhoneMessage
                {
                    sender = null,
                    text = choices[choiceIndex].label
                };

                MessageBubbleView choiceView = Instantiate(bubblePrefab, messagesContainer);
                choiceView.SetupInstant(choiceBubble, protagonist.playerName);
                _bubbles.Add(choiceView);
            }

            advanceButton.gameObject.SetActive(true);
            phonePanel.SetActive(true);

            StartCoroutine(ScrollToBottomImmediate());
        }

    }
}
