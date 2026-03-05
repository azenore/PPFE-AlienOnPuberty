using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class ChapterManager : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private DialogueChapter startingChapter;

        private DialogueChapter _currentChapter;

        /// <summary>Asset name of the active chapter, used when building a save snapshot.</summary>
        public string CurrentChapterName => _currentChapter != null ? _currentChapter.name : string.Empty;

        private void Start()
        {
            engine.OnChapterFinished += HandleChapterFinished;
        }

        private void OnDestroy()
        {
            engine.OnChapterFinished -= HandleChapterFinished;
        }

        /// <summary>Appelé par CharacterCustomizationController après confirmation.</summary>
        public void StartGame()
        {
            LoadChapter(startingChapter);
        }

        /// <summary>Loads a specific chapter into the engine from its first node.</summary>
        public void LoadChapter(DialogueChapter chapter)
        {
            if (chapter == null)
            {
                Debug.Log("[ChapterManager] Histoire terminée.");
                return;
            }

            _currentChapter = chapter;
            engine.LoadChapter(chapter);
        }

        /// <summary>Loads a chapter and resumes at a specific node index. Used when restoring a save.</summary>
        public void LoadChapterAtLine(DialogueChapter chapter, int lineIndex)
        {
            if (chapter == null) return;
            _currentChapter = chapter;
            engine.LoadChapterAtLine(chapter, lineIndex);
        }

        private void HandleChapterFinished(DialogueChapter nextFromChoice)
        {
            if (nextFromChoice != null)
            {
                LoadChapter(nextFromChoice);
                return;
            }

            LoadChapter(_currentChapter.defaultNextChapter);
        }
    }
}
