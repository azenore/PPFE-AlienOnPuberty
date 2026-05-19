using System;
using System.Collections.Generic;
using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class DialogueEngine : MonoBehaviour
    {
        [SerializeField] private AffinitySystem affinitySystem;

        private List<DialogueNode> _nodes;
        private int _currentIndex;
        private bool _waitingForChoice;

        public event Action<DialogueLine> OnLineReady;
        public event Action<List<DialogueChoice>> OnChoiceReady;
        public event Action<BaseChapter> OnChapterFinished;
        public event Action<Sprite> OnBackgroundChanged;
        public event Action<CharacterData, EmotionType> OnCharacterOnScreenChanged;
        public event Action<EmotionType> OnProtagonistEmotionChanged;
        public event Action<bool> OnMonologueStateChanged;

        /// <summary>Index of the last displayed node. Use this for saving.</summary>
        public int LastDisplayedIndex => Mathf.Max(0, _currentIndex - 1);

        /// <summary>Last character displayed on screen. Used to restore UI state after Continue.</summary>
        public CharacterData CurrentCharacter { get; private set; }

        /// <summary>Whether the current node is a monologue.</summary>
        public bool IsMonologue { get; private set; }

        /// <summary>Loads a chapter and starts from the first node.</summary>
        public void LoadChapter(DialogueChapter chapter) => LoadChapterAtLine(chapter, 0);

        /// <summary>Loads a chapter and resumes at a specific node index.</summary>
        public void LoadChapterAtLine(DialogueChapter chapter, int startIndex)
        {
            _nodes = chapter.nodes;
            _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _nodes.Count - 1));
            _waitingForChoice = false;

            DisplayNodeAt(_currentIndex);
        }

        /// <summary>
        /// Restores the last visible character without firing events.
        /// Must be called before LoadChapterAtLine when loading a save.
        /// </summary>
        public void RestoreCharacter(CharacterData character) => CurrentCharacter = character;

        /// <summary>Advances to the next node. Call this on player input.</summary>
        public void Advance()
        {
            if (_waitingForChoice) return;
            DisplayNodeAt(_currentIndex);
        }

        /// <summary>Resolves a player choice, applies affinity and signals chapter transition.</summary>
        public void SelectChoice(DialogueChoice choice)
        {
            if (!_waitingForChoice) return;
            _waitingForChoice = false;
            affinitySystem.ApplyChoiceAffinity(choice);
            OnChapterFinished?.Invoke(choice.nextChapter);
        }

        private void DisplayNodeAt(int index)
        {
            if (index >= _nodes.Count)
            {
                SetMonologue(false);
                OnChapterFinished?.Invoke(null);
                return;
            }

            DialogueNode node = _nodes[index];
            _currentIndex = index + 1;

            if (node.backgroundOverride != null)
                OnBackgroundChanged?.Invoke(node.backgroundOverride);

            // Met à jour le personnage courant même pendant un monologue,
            // pour qu'il soit prêt à l'affichage dès la fin du monologue.
            if (node.characterOnScreen != null)
                CurrentCharacter = node.characterOnScreen;

            SetMonologue(node.isMonologue);

            if (!node.isMonologue)
            {
                if (CurrentCharacter != null)
                    OnCharacterOnScreenChanged?.Invoke(CurrentCharacter, node.characterOnScreen != null ? node.characterOnScreenEmotion : EmotionType.Neutral);
            }

            if (node.overrideProtagonistEmotion)
                OnProtagonistEmotionChanged?.Invoke(node.protagonistEmotion);

            if (node.IsChoiceNode)
            {
                _waitingForChoice = true;
                OnChoiceReady?.Invoke(node.choices);
            }
            else
            {
                OnLineReady?.Invoke(new DialogueLine
                {
                    speaker = node.speaker,
                    isProtagonist = node.speaker == null,
                    text = node.text,
                    voiceClip = node.voiceClip
                });


            }
        }

        private void SetMonologue(bool monologue)
        {
            if (IsMonologue == monologue) return;
            IsMonologue = monologue;
            OnMonologueStateChanged?.Invoke(monologue);
        }
    }
}
