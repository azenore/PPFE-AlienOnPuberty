using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class PhoneChatController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PhoneEngine phoneEngine;
        [SerializeField] private ProtagonistData protagonist;

        [Header("UI")]
        [SerializeField] private GameObject phonePanel;
        [SerializeField] private Transform messagesContainer;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private MessageBubbleView bubblePrefab;
        [SerializeField] private Button advanceButton;

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

        /// <summary>Shows the phone panel and clears previous messages.</summary>
        public void OpenChat()
        {
            ClearBubbles();
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

            // Scroll vers le bas au prochain frame pour laisser le layout se recalculer
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void HandleConversationFinished(DialogueChapter next)
        {
            CloseChat();
            // ChapterManager prend le relais via son event
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
