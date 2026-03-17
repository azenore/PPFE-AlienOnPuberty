using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class AvanceGame : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private ChapterManager chapterManager;
        [SerializeField] private Button advanceButton;

        private void OnEnable()
        {
            engine.OnChoiceReady += HandleChoiceReady;
            engine.OnLineReady += HandleLineReady;
            engine.OnChapterFinished += HandleChapterFinished;
            chapterManager.OnPhoneChapterStarted += HandlePhoneChapterStarted;
        }

        private void OnDisable()
        {
            engine.OnChoiceReady -= HandleChoiceReady;
            engine.OnLineReady -= HandleLineReady;
            engine.OnChapterFinished -= HandleChapterFinished;
            chapterManager.OnPhoneChapterStarted -= HandlePhoneChapterStarted;
        }

        /// <summary>Called by the advance button's OnClick event.</summary>
        public void Advance()
        {
            engine.Advance();
        }

        private void HandleChoiceReady(System.Collections.Generic.List<DialogueChoice> _)
            => advanceButton.interactable = false;

        private void HandleLineReady(DialogueLine _)
            => advanceButton.interactable = true;

        private void HandleChapterFinished(DialogueChapter _)
            => advanceButton.interactable = true;

        private void HandlePhoneChapterStarted()
            => advanceButton.interactable = false;
    }
}
