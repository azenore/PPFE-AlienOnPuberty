using UnityEngine;
using VN.Data;

namespace VN.Runtime
{
    public class ChapterManager : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private DialogueChapter startingChapter;

        private DialogueChapter _currentChapter;

        private void Start()
        {
            engine.OnChapterFinished += HandleChapterFinished;
            LoadChapter(startingChapter);
        }

        private void OnDestroy()
        {
            engine.OnChapterFinished -= HandleChapterFinished;
        }

        /// <summary>Loads a specific chapter into the engine.</summary>
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
