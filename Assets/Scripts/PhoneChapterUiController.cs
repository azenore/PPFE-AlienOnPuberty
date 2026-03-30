using System.Collections;
using UnityEngine;
using VN.Runtime;

namespace VN.UI
{
    /// <summary>
    /// Masque les éléments de jeu pendant un chapitre téléphone et les restaure ensuite.
    /// Anime le PhonePanel avec un slide vers le haut à l'entrée et vers le bas à la sortie.
    /// Attacher sur GameManager.
    /// </summary>
    public class PhoneChapterUIController : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private ChapterManager chapterManager;

        [Header("Phone Panel")]
        [SerializeField] private RectTransform phonePanelRect;
        [SerializeField] private float slideDuration = 0.4f;
        [SerializeField] private float slideDistance = 1200f;
        [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Éléments à masquer pendant un chapitre téléphone")]
        [SerializeField] private GameObject npcPortrait;
        [SerializeField] private GameObject protagonistPortrait;
        [SerializeField] private GameObject dialogueBox;
        [SerializeField] private GameObject advanceButton;
        [SerializeField] private GameObject choiceMenu;
        [SerializeField] private GameObject affinityBarPanel;

        private Vector2 _restPosition;
        private Vector2 _offscreenPosition;
        private Coroutine _slideCoroutine;

        private void Awake()
        {
            _restPosition = phonePanelRect.anchoredPosition;
            _offscreenPosition = new Vector2(_restPosition.x, _restPosition.y - slideDistance);

            phonePanelRect.anchoredPosition = _offscreenPosition;
            phonePanelRect.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            chapterManager.OnPhoneChapterStarted += OnPhoneStarted;
            chapterManager.OnPhoneChapterEnded += OnPhoneEnded;
            chapterManager.OnDialogueChapterLoaded += ResetToDialogueMode;
        }

        private void OnDisable()
        {
            chapterManager.OnPhoneChapterStarted -= OnPhoneStarted;
            chapterManager.OnPhoneChapterEnded -= OnPhoneEnded;
            chapterManager.OnDialogueChapterLoaded -= ResetToDialogueMode;
        }

        private void OnPhoneStarted()
        {
            SetDialogueUIVisible(false);

            if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
            _slideCoroutine = StartCoroutine(SlideIn());
        }

        private void OnPhoneEnded()
        {
            if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
            _slideCoroutine = StartCoroutine(SlideOut());
        }

        private void ResetToDialogueMode()
        {
            if (_slideCoroutine != null) StopCoroutine(_slideCoroutine);
            phonePanelRect.gameObject.SetActive(false);
            SetDialogueUIVisible(true);
        }

        private IEnumerator SlideIn()
        {
            phonePanelRect.anchoredPosition = _offscreenPosition;
            phonePanelRect.gameObject.SetActive(true);
            yield return Slide(_offscreenPosition, _restPosition);
        }

        private IEnumerator SlideOut()
        {
            phonePanelRect.gameObject.SetActive(true);
            phonePanelRect.anchoredPosition = _restPosition;

            yield return Slide(_restPosition, _offscreenPosition);

            phonePanelRect.gameObject.SetActive(false);
            SetDialogueUIVisible(true);

            chapterManager.OnPhoneExitAnimationComplete();
        }

        private IEnumerator Slide(Vector2 from, Vector2 to)
        {
            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                elapsed += Time.deltaTime;
                float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / slideDuration));
                phonePanelRect.anchoredPosition = Vector2.Lerp(from, to, t);
                yield return null;
            }
            phonePanelRect.anchoredPosition = to;
        }

        private void SetDialogueUIVisible(bool visible)
        {
            npcPortrait.SetActive(visible);
            protagonistPortrait.SetActive(visible);
            dialogueBox.SetActive(visible);
            advanceButton.SetActive(visible);
            choiceMenu.SetActive(visible);
            affinityBarPanel.SetActive(visible);
        }
    }
}
