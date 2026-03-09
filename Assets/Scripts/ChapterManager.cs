using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class ChapterManager : MonoBehaviour
    {
        [Header("Engines")]
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private PhoneEngine phoneEngine;

        [Header("Chapters")]
        [SerializeField] private DialogueChapter startingChapter;

        [Header("UI")]
        [SerializeField] private VN.UI.PhoneChatController phoneChatController;
        [SerializeField] private GameObject gamePanel;

        private DialogueChapter _currentChapter;
        private PhoneChapter _currentPhoneChapter;
        private bool _inPhoneChapter;

        /// <summary>Asset name of the active dialogue chapter. Used when building a save snapshot.</summary>
        public string CurrentChapterName => _currentChapter != null ? _currentChapter.name : string.Empty;

        /// <summary>True if currently in a phone chapter.</summary>
        public bool IsInPhoneChapter => _inPhoneChapter;

        /// <summary>Asset name of the active phone chapter. Used when building a save snapshot.</summary>
        public string CurrentPhoneChapterName => _currentPhoneChapter != null ? _currentPhoneChapter.name : string.Empty;

        private void Start()
        {
            engine.OnChapterFinished += HandleDialogueChapterFinished;
            phoneEngine.OnConversationFinished += HandlePhoneConversationFinished;
            phoneEngine.OnPhoneChapterFinished += HandleNextPhoneChapter;
        }

        private void OnDestroy()
        {
            engine.OnChapterFinished -= HandleDialogueChapterFinished;
            phoneEngine.OnConversationFinished -= HandlePhoneConversationFinished;
            phoneEngine.OnPhoneChapterFinished -= HandleNextPhoneChapter;
        }

        /// <summary>Called by CharacterCustomizationController after confirmation.</summary>
        public void StartGame()
        {
            LoadChapter(startingChapter);
        }

        /// <summary>Loads a dialogue chapter from its first node.</summary>
        public void LoadChapter(DialogueChapter chapter)
        {
            if (chapter == null)
            {
                Debug.Log("[ChapterManager] Histoire terminée.");
                return;
            }

            _inPhoneChapter = false;
            _currentChapter = chapter;
            phoneChatController.CloseChat();
            gamePanel.SetActive(true);
            engine.LoadChapter(chapter);
        }

        /// <summary>Loads a dialogue chapter at a specific node. Used when restoring a save.</summary>
        public void LoadChapterAtLine(DialogueChapter chapter, int lineIndex)
        {
            if (chapter == null) return;
            _inPhoneChapter = false;
            _currentChapter = chapter;
            phoneChatController.CloseChat();
            gamePanel.SetActive(true);
            engine.LoadChapterAtLine(chapter, lineIndex);
        }

        /// <summary>Loads a phone chapter.</summary>
        public void LoadPhoneChapter(PhoneChapter chapter)
        {
            if (chapter == null) return;
            _inPhoneChapter = true;
            _currentPhoneChapter = chapter;
            phoneChatController.OpenChat(chapter);  // ← correction
            phoneEngine.LoadPhoneChapter(chapter);
        }

        private void HandleDialogueChapterFinished(DialogueChapter nextFromChoice)
        {
            if (nextFromChoice != null)
            {
                LoadChapter(nextFromChoice);
                return;
            }

            if (_currentChapter.nextPhoneChapter != null)
            {
                LoadPhoneChapter(_currentChapter.nextPhoneChapter);
                return;
            }

            LoadChapter(_currentChapter.defaultNextChapter);
        }

        private void HandlePhoneConversationFinished(DialogueChapter next)
        {
            _inPhoneChapter = false;
            LoadChapter(next);
        }

        private void HandleNextPhoneChapter(PhoneChapter next)
        {
            _currentPhoneChapter = next;
            phoneChatController.OpenChat(next);     // ← correction
            phoneEngine.LoadPhoneChapter(next);
        }
    }
}
