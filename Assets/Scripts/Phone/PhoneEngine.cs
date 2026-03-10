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
        private DialogueChapter _storedDialogueNext;
        private PhoneChapter _storedPhoneNext;
        private readonly Queue<PhoneMessage> _followUpQueue = new();

        public event Action<PhoneMessage> OnMessageReady;
        public event Action<List<PhoneChoice>> OnChoiceReady;
        public event Action<DialogueChapter> OnConversationFinished;
        public event Action<PhoneChapter> OnPhoneChapterFinished;

        /// <summary>Index of the last revealed message. Use this for saving.</summary>
        public int LastRevealedIndex => Mathf.Max(0, _currentIndex - 1);

        /// <summary>Index of the message that triggered the last choice. -1 if none. Use this for saving.</summary>
        public int ChoiceMessageIndex => _choiceMessageIndex;

        /// <summary>Index of the selected choice in the last choice message. -1 if none. Use this for saving.</summary>
        public int SelectedChoiceIndex => _selectedChoiceIndex;

        /// <summary>Loads a phone chapter and reveals the first message.</summary>
        public void LoadPhoneChapter(PhoneChapter chapter)
        {
            LoadPhoneChapterAtMessage(chapter, 0);
        }

        /// <summary>Loads a phone chapter and reveals from a specific index. Used for normal navigation.</summary>
        public void LoadPhoneChapterAtMessage(PhoneChapter chapter, int startIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _messages.Count - 1));
            _storedDialogueNext = chapter.defaultNextChapter;
            _storedPhoneNext = chapter.defaultNextPhoneChapter;
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            _followUpQueue.Clear();

            RevealNext();
        }

        /// <summary>
        /// Positions the engine at resumeFromIndex WITHOUT revealing any message.
        /// If the last replayed message had choices, re-fires OnChoiceReady so the UI restores correctly.
        /// </summary>
        public void RestoreAtMessage(PhoneChapter chapter, int resumeFromIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(resumeFromIndex, 0, _messages.Count);
            _storedDialogueNext = chapter.defaultNextChapter;
            _storedPhoneNext = chapter.defaultNextPhoneChapter;
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            _followUpQueue.Clear();

            // Si le dernier message affiché avait des choix, on les remet en attente
            int lastIndex = resumeFromIndex - 1;
            if (lastIndex >= 0 && lastIndex < _messages.Count)
            {
                PhoneMessage lastMessage = _messages[lastIndex];
                if (lastMessage.HasChoices)
                    OnChoiceReady?.Invoke(lastMessage.choices);
            }
        }

        /// <summary>Reveals the next message. Call this on player tap.</summary>
        public void Advance()
        {
            if (_followUpQueue.Count > 0)
            {
                OnMessageReady?.Invoke(_followUpQueue.Dequeue());
                return;
            }

            // On quitte la phase de choix en avançant vers le message suivant
            _selectedChoiceIndex = -1;
            _choiceMessageIndex = -1;
            RevealNext();
        }

        /// <summary>Shows the choice as a protagonist bubble, then queues follow-up messages.</summary>
        public void SelectChoice(PhoneChoice choice, AffinitySystem affinitySystem)
        {
            // Mémorise l'index du message porteur du choix et l'index du choix sélectionné
            _choiceMessageIndex = _currentIndex - 1;
            if (_choiceMessageIndex >= 0 && _choiceMessageIndex < _messages.Count)
                _selectedChoiceIndex = _messages[_choiceMessageIndex].choices.IndexOf(choice);

            if (choice.affinityTarget != null)
                affinitySystem.ApplyDelta(choice.affinityTarget, choice.affinityDelta);

            if (choice.nextPhoneChapter != null) _storedPhoneNext = choice.nextPhoneChapter;
            else if (choice.nextChapter != null) _storedDialogueNext = choice.nextChapter;

            OnMessageReady?.Invoke(new PhoneMessage { sender = null, text = choice.label });

            _followUpQueue.Clear();
            foreach (var msg in choice.followUpMessages)
                _followUpQueue.Enqueue(msg);
        }

        private void RevealNext()
        {
            if (_currentIndex >= _messages.Count)
            {
                if (_storedPhoneNext != null)
                    OnPhoneChapterFinished?.Invoke(_storedPhoneNext);
                else
                    OnConversationFinished?.Invoke(_storedDialogueNext);
                return;
            }

            PhoneMessage msg = _messages[_currentIndex];
            _currentIndex++;
            OnMessageReady?.Invoke(msg);

            if (msg.HasChoices)
                OnChoiceReady?.Invoke(msg.choices);
        }

        public void RestoreAfterChoice(PhoneChapter chapter, int choiceMessageIndex, int choiceIndex)
        {
            _messages = chapter.messages;
            _currentIndex = Mathf.Clamp(choiceMessageIndex + 1, 0, _messages.Count);
            _storedDialogueNext = chapter.defaultNextChapter;
            _storedPhoneNext = chapter.defaultNextPhoneChapter;
            _selectedChoiceIndex = choiceIndex;
            _choiceMessageIndex = choiceMessageIndex;
            _followUpQueue.Clear();

            if (choiceMessageIndex >= 0 && choiceMessageIndex < _messages.Count)
            {
                List<PhoneChoice> choices = _messages[choiceMessageIndex].choices;
                if (choiceIndex >= 0 && choiceIndex < choices.Count)
                {
                    PhoneChoice choice = choices[choiceIndex];
                    if (choice.nextPhoneChapter != null) _storedPhoneNext = choice.nextPhoneChapter;
                    else if (choice.nextChapter != null) _storedDialogueNext = choice.nextChapter;

                    // Restaure les follow-ups pour que Advance() les livre dans l'ordre
                    foreach (var msg in choice.followUpMessages)
                        _followUpQueue.Enqueue(msg);
                }
            }
        }

    }

}
