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
        public event Action<DialogueChapter> OnChapterFinished;
        public event Action<Sprite> OnBackgroundChanged;
        public event Action<CharacterData, EmotionType> OnCharacterOnScreenChanged;
        public event Action<EmotionType> OnProtagonistEmotionChanged;

        /// <summary>Index of the last displayed node. Use this for saving.</summary>
        public int LastDisplayedIndex => Mathf.Max(0, _currentIndex - 1);

        /// <summary>Last character displayed on screen. Used to restore UI state after Continue.</summary>
        public CharacterData CurrentCharacter { get; private set; }

        /// <summary>Loads a chapter and starts from the first node.</summary>
        public void LoadChapter(DialogueChapter chapter)
        {
            LoadChapterAtLine(chapter, 0);
        }

        /// <summary>Loads a chapter and resumes at a specific node index.</summary>
        public void LoadChapterAtLine(DialogueChapter chapter, int startIndex)
        {
            _nodes = chapter.nodes;

            // Clamp à un index toujours valide — évite de déclencher OnChapterFinished au restore
            int clampedIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _nodes.Count - 1));
            _currentIndex = clampedIndex;
            _waitingForChoice = false;

            if (chapter.background != null)
                OnBackgroundChanged?.Invoke(chapter.background);

            DisplayNodeAt(_currentIndex);
        }

        /// <summary>
        /// Restores the last visible character without firing events.
        /// Must be called before LoadChapterAtLine when loading a save.
        /// </summary>
        public void RestoreCharacter(CharacterData character)
        {
            CurrentCharacter = character;
        }

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
                OnChapterFinished?.Invoke(null);
                return;
            }

            DialogueNode node = _nodes[index];
            _currentIndex = index + 1;

            if (node.backgroundOverride != null)
                OnBackgroundChanged?.Invoke(node.backgroundOverride);

            if (node.characterOnScreen != null)
            {
                // Le nœud définit explicitement un nouveau personnage
                CurrentCharacter = node.characterOnScreen;
                OnCharacterOnScreenChanged?.Invoke(node.characterOnScreen, node.characterOnScreenEmotion);
            }
            else if (CurrentCharacter != null)
            {
                // Le personnage persiste depuis un nœud précédent — notifie quand même l'UI pour le restore
                OnCharacterOnScreenChanged?.Invoke(CurrentCharacter, EmotionType.Neutral);
            }

            if (node.overrideProtagonistEmotion)
                OnProtagonistEmotionChanged?.Invoke(node.protagonistEmotion);

            if (node.isChoiceNode)
            {
                _waitingForChoice = true;
                OnChoiceReady?.Invoke(node.choices);
            }
            else
            {
                OnLineReady?.Invoke(node.line);
            }
        }
    }
}
