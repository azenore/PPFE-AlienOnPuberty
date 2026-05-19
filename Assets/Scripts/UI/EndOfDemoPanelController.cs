using System.Collections;
using UnityEngine;

namespace VN.UI
{
    /// <summary>
    /// Affiche le panneau de fin de démo avec un fondu d'apparition.
    /// Appeler Show() quand il n'y a plus de chapitre à charger.
    /// </summary>
    public class EndOfDemoPanelController : MonoBehaviour
    {
        private const float FadeDuration = 0.8f;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeIn());
        }

        /// <summary>Active le panneau et le fait apparaître en fondu.</summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        private IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < FadeDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / FadeDuration));
                yield return null;
            }
            _canvasGroup.alpha = 1f;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }
}
