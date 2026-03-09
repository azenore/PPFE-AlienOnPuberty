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
            phoneEngine.OnConversationFinished += HandleConversationFinished;
            phoneEngine.OnPhoneChapterFinished += HandlePhoneChapterFinished;
        }

        private void OnDisable()
        {
            phoneEngine.OnMessageReady -= HandleMessageReady;
            phoneEngine.OnConversationFinished -= HandleConversationFinished;
            phoneEngine.OnPhoneChapterFinished -= HandlePhoneChapterFinished;
        }

        /// <summary>Shows the phone panel, clears previous messages and updates the header.</summary>
        public void OpenChat(PhoneChapter chapter)
        {
            ClearBubbles();
            if (participantsText != null)
                participantsText.text = chapter.GetHeaderLabel();
            phonePanel.SetActive(true);
        }

        /// <summary>Hides the phone panel.</summary>
        public void CloseChat()
        {
            phonePanel.SetActive(false);
        }

        private void HandleMessageReady(PhoneMessage message)
        {
            MessageBubbleView bubble = Instantiate(bubblePrefab, messagesContainer);
            bubble.Setup(message, protagonist.playerName);
            _bubbles.Add(bubble);

            StartCoroutine(ScrollToBottom());
        }

        private IEnumerator ScrollToBottom()
        {
            yield return new WaitForSeconds(BubbleAnimDuration);
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
    }
}
