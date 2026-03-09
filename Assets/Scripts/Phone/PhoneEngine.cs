using System;
using System.Collections.Generic;
using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class PhoneEngine : MonoBehaviour
    {
        private List<PhoneMessage> _messages;
        private int _currentIndex;

        public event Action<PhoneMessage> OnMessageReady;
        public event Action<DialogueChapter> OnConversationFinished;
        public event Action<PhoneChapter> OnPhoneChapterFinished;

        /// <summary>Index of the last revealed message. Use this for saving.</summary>
        public int LastRevealedIndex => Mathf.Max(0, _currentIndex - 1);

        /// <summary>Loads a phone chapter and reveals the first message.</summary>
        public void LoadPhoneChapter(PhoneChapter chapter)
        {
            LoadPhoneChapterAtMessage(chapter, 0);
        }

        /// <summary>Loads a phone chapter and resumes at a specific message index.</summary>
        public void LoadPhoneChapterAtMessage(PhoneChapter chapter, int startIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _messages.Count - 1));

            _storedDialogueNext = chapter.defaultNextChapter;
            _storedPhoneNext = chapter.defaultNextPhoneChapter;

            RevealMessageAt(_currentIndex);
        }

        /// <summary>Reveals the next message. Call this on player tap.</summary>
        public void Advance()
        {
            RevealMessageAt(_currentIndex);
        }

        private DialogueChapter _storedDialogueNext;
        private PhoneChapter _storedPhoneNext;

        private void RevealMessageAt(int index)
        {
            if (index >= _messages.Count)
            {
                // Conversation terminée — notifie selon le type de chapitre suivant
                if (_storedPhoneNext != null)
                    OnPhoneChapterFinished?.Invoke(_storedPhoneNext);
                else
                    OnConversationFinished?.Invoke(_storedDialogueNext);
                return;
            }

            _currentIndex = index + 1;
            OnMessageReady?.Invoke(_messages[index]);
        }
    }
}
