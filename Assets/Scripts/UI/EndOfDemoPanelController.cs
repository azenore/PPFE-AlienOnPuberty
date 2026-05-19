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

        [Header("Références")]
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
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
                canvasGroup.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / FadeDuration));
                yield return null;
            }
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
