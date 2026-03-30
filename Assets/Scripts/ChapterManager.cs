using System;
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
        [SerializeField] private ProtagonistData protagonist;

        [Header("UI")]
        [SerializeField] private VN.UI.PhoneChatController phoneChatController;
        [SerializeField] private GameObject gamePanel;

        private DialogueChapter _currentChapter;
        private PhoneChapter _currentPhoneChapter;
        private BaseChapter _pendingChapterAfterPhone;
        private bool _inPhoneChapter;

        /// <summary>Asset name of the active dialogue chapter. Used when building a save snapshot.</summary>
        public string CurrentChapterName => _currentChapter != null ? _currentChapter.name : string.Empty;

        /// <summary>True if currently in a phone chapter.</summary>
        public bool IsInPhoneChapter => _inPhoneChapter;

        /// <summary>Asset name of the active phone chapter. Used when building a save snapshot.</summary>
        public string CurrentPhoneChapterName => _currentPhoneChapter != null ? _currentPhoneChapter.name : string.Empty;

        /// <summary>Fired when a phone chapter starts.</summary>
        public event Action OnPhoneChapterStarted;

        /// <summary>Fired when a phone chapter ends. LoadNextChapter is deferred until OnPhoneExitAnimationComplete is called.</summary>
        public event Action OnPhoneChapterEnded;

        /// <summary>Fired when a dialogue chapter is loaded. Allows UI to restore dialogue elements hidden during a phone chapter.</summary>
        public event Action OnDialogueChapterLoaded;

        private void Start()
        {
            engine.OnChapterFinished += HandleDialogueChapterFinished;
            phoneEngine.OnChapterFinished += HandlePhoneChapterFinished;
        }

        private void OnDestroy()
        {
            engine.OnChapterFinished -= HandleDialogueChapterFinished;
            phoneEngine.OnChapterFinished -= HandlePhoneChapterFinished;
        }

        /// <summary>Called by CharacterCustomizationController after confirmation.</summary>
        public void StartGame() => LoadChapter(startingChapter);

        /// <summary>Loads a dialogue chapter from its first node.</summary>
        public void LoadChapter(DialogueChapter chapter)
        {
            if (chapter == null) { Debug.Log("[ChapterManager] Histoire terminée."); return; }
            _inPhoneChapter = false;
            _currentChapter = chapter;
            phoneChatController.CloseChat();
            gamePanel.SetActive(true);
            OnDialogueChapterLoaded?.Invoke();
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
            OnDialogueChapterLoaded?.Invoke();
            engine.LoadChapterAtLine(chapter, lineIndex);
        }

        /// <summary>
        /// Silently stores the current dialogue chapter without triggering the engine.
        /// Used when restoring a save during a phone chapter.
        /// </summary>
        public void SetCurrentChapterSilent(DialogueChapter chapter) => _currentChapter = chapter;

        /// <summary>Loads a phone chapter from its first message.</summary>
        public void LoadPhoneChapter(PhoneChapter chapter)
        {
            if (chapter == null) return;
            _inPhoneChapter = true;
            _currentPhoneChapter = chapter;
            phoneChatController.OpenChat(chapter);
            phoneEngine.LoadPhoneChapter(chapter);
            OnPhoneChapterStarted?.Invoke();
        }

        /// <summary>Restores a phone chapter at a saved message index.</summary>
        public void LoadPhoneChapterAtMessage(PhoneChapter chapter, int messageIndex)
        {
            if (chapter == null) return;
            _inPhoneChapter = true;
            _currentPhoneChapter = chapter;
            gamePanel.SetActive(true);
            phoneChatController.OpenChatWithReplay(chapter, messageIndex);
            phoneEngine.RestoreAtMessage(chapter, messageIndex + 1);
            OnPhoneChapterStarted?.Invoke();
        }

        /// <summary>Restores a phone chapter after a choice was made at save time.</summary>
        public void LoadPhoneChapterAfterChoice(PhoneChapter chapter, int choiceMessageIndex, int choiceIndex)
        {
            if (chapter == null) return;
            _inPhoneChapter = true;
            _currentPhoneChapter = chapter;
            gamePanel.SetActive(true);
            phoneChatController.OpenChatWithReplay(chapter, choiceMessageIndex, choiceIndex);
            phoneEngine.RestoreAfterChoice(chapter, choiceMessageIndex, choiceIndex);
            OnPhoneChapterStarted?.Invoke();
        }

        /// <summary>Called by PhoneChapterUIController once the exit animation is complete.</summary>
        public void OnPhoneExitAnimationComplete()
        {
            var pending = _pendingChapterAfterPhone;
            _pendingChapterAfterPhone = null;
            LoadNextChapter(pending);
        }

        private void HandleDialogueChapterFinished(BaseChapter nextFromChoice)
        {
            if (nextFromChoice != null) { LoadNextChapter(nextFromChoice); return; }

            DialogueChapter unlocked = _currentChapter.GetUnlockedChapter(protagonist);
            if (unlocked != null) { LoadChapter(unlocked); return; }

            LoadNextChapter(_currentChapter.transition.nextChapter);
        }

        private void HandlePhoneChapterFinished(BaseChapter next)
        {
            _inPhoneChapter = false;
            _currentPhoneChapter = null;
            _pendingChapterAfterPhone = next;
            OnPhoneChapterEnded?.Invoke();
        }

        private void LoadNextChapter(BaseChapter next)
        {
            switch (next)
            {
                case PhoneChapter phone: LoadPhoneChapter(phone); break;
                case DialogueChapter dial: LoadChapter(dial); break;
                default: Debug.Log("[ChapterManager] Histoire terminée."); break;
            }
        }
    }
}
