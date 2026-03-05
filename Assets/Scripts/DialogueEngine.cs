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

        /// <summary>Loads a chapter and starts from the first node.</summary>
        public void LoadChapter(DialogueChapter chapter)
        {
            LoadChapterAtLine(chapter, 0);
        }

        /// <summary>Loads a chapter and resumes at a specific node index.</summary>
        public void LoadChapterAtLine(DialogueChapter chapter, int startIndex)
        {
            _nodes = chapter.nodes;
            _currentIndex = Mathf.Clamp(startIndex, 0, Mathf.Max(0, _nodes.Count - 1));
            _waitingForChoice = false;

            if (chapter.background != null)
                OnBackgroundChanged?.Invoke(chapter.background);

            Advance();
        }

        /// <summary>Advances to the next node. Call this on player input (click/tap).</summary>
        public void Advance()
        {
            if (_waitingForChoice) return;

            if (_currentIndex >= _nodes.Count)
            {
                OnChapterFinished?.Invoke(null);
                return;
            }

            DialogueNode node = _nodes[_currentIndex];
            _currentIndex++;

            if (node.backgroundOverride != null)
                OnBackgroundChanged?.Invoke(node.backgroundOverride);

            if (node.characterOnScreen != null)
                OnCharacterOnScreenChanged?.Invoke(node.characterOnScreen, node.characterOnScreenEmotion);

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

        /// <summary>Resolves a player choice, applies affinity and signals chapter transition.</summary>
        public void SelectChoice(DialogueChoice choice)
        {
            if (!_waitingForChoice) return;

            _waitingForChoice = false;
            affinitySystem.ApplyChoiceAffinity(choice);
            OnChapterFinished?.Invoke(choice.nextChapter);
        }
    }
}
