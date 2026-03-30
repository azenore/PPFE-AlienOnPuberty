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
        private int _selectedChoiceIndex = -1;
        private int _choiceMessageIndex = -1;
        private BaseChapter _storedNext;
        private readonly Queue<PhoneMessage> _followUpQueue = new();

        public event Action<PhoneMessage> OnMessageReady;
        public event Action<List<PhoneChoice>> OnChoiceReady;

        /// <summary>Fired when the conversation ends. Payload is the next chapter (dialogue or phone).</summary>
        public event Action<BaseChapter> OnChapterFinished;

        /// <summary>Index of the last revealed message. Use this for saving.</summary>
        public int LastRevealedIndex => Mathf.Max(0, _currentIndex - 1);

        /// <summary>Index of the message that triggered the last choice. -1 if none.</summary>
        public int ChoiceMessageIndex => _choiceMessageIndex;

        /// <summary>Index of the selected choice in the last choice message. -1 if none.</summary>
        public int SelectedChoiceIndex => _selectedChoiceIndex;

        /// <summary>Loads a phone chapter and reveals the first message.</summary>
        public void LoadPhoneChapter(PhoneChapter chapter) => LoadPhoneChapterAtMessage(chapter, 0);

        /// <summary>Loads a phone chapter and reveals from a specific index.</summary>
        public void LoadPhoneChapterAtMessage(PhoneChapter chapter, int startIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _messages.Count - 1));
            _storedNext = chapter.transition.nextChapter;
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            _followUpQueue.Clear();
            RevealNext();
        }

        /// <summary>
        /// Positions the engine at resumeFromIndex WITHOUT revealing any message.
        /// Re-fires OnChoiceReady if the last replayed message had choices.
        /// </summary>
        public void RestoreAtMessage(PhoneChapter chapter, int resumeFromIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(resumeFromIndex, 0, _messages.Count);
            _storedNext = chapter.transition.nextChapter;
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            _followUpQueue.Clear();

            int lastIndex = resumeFromIndex - 1;
            if (lastIndex >= 0 && lastIndex < _messages.Count && _messages[lastIndex].HasChoices)
                OnChoiceReady?.Invoke(_messages[lastIndex].choices);
        }

        /// <summary>Reveals the next message. Call this on player tap.</summary>
        public void Advance()
        {
            if (_followUpQueue.Count > 0) { OnMessageReady?.Invoke(_followUpQueue.Dequeue()); return; }
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            RevealNext();
        }

        /// <summary>Shows the choice as a protagonist bubble, then queues follow-up messages.</summary>
        public void SelectChoice(PhoneChoice choice, AffinitySystem affinitySystem)
        {
            _choiceMessageIndex = _currentIndex - 1;
            if (_choiceMessageIndex >= 0 && _choiceMessageIndex < _messages.Count)
                _selectedChoiceIndex = _messages[_choiceMessageIndex].choices.IndexOf(choice);

            if (choice.affinityTarget != null)
                affinitySystem.ApplyDelta(choice.affinityTarget, choice.affinityDelta);

            if (choice.nextChapter != null)
                _storedNext = choice.nextChapter;

            OnMessageReady?.Invoke(new PhoneMessage { sender = null, text = choice.label });

            _followUpQueue.Clear();
            foreach (var msg in choice.followUpMessages)
                _followUpQueue.Enqueue(msg);
        }

        /// <summary>Restores engine state after a choice was made at save time.</summary>
        public void RestoreAfterChoice(PhoneChapter chapter, int choiceMessageIndex, int choiceIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(choiceMessageIndex + 1, 0, _messages.Count);
            _storedNext = chapter.transition.nextChapter;
            _selectedChoiceIndex = choiceIndex;
            _choiceMessageIndex = choiceMessageIndex;
            _followUpQueue.Clear();

            if (choiceMessageIndex < 0 || choiceMessageIndex >= _messages.Count) return;

            List<PhoneChoice> choices = _messages[choiceMessageIndex].choices;
            if (choiceIndex < 0 || choiceIndex >= choices.Count) return;

            if (choices[choiceIndex].nextChapter != null)
                _storedNext = choices[choiceIndex].nextChapter;

            foreach (var msg in choices[choiceIndex].followUpMessages)
                _followUpQueue.Enqueue(msg);
        }

        private void RevealNext()
        {
            if (_currentIndex >= _messages.Count)
            {
                OnChapterFinished?.Invoke(_storedNext);
                return;
            }

            PhoneMessage msg = _messages[_currentIndex++];
            OnMessageReady?.Invoke(msg);
            if (msg.HasChoices) OnChoiceReady?.Invoke(msg.choices);
        }
    }
}
