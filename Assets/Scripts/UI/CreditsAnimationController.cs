using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace VN.UI
{
    /// <summary>
    /// Anime la soucoupe crédits : slide en diagonale avec rotation vers la position ouverte,
    /// fait apparaître le rayon en fondu puis le texte. Second clic = fermeture inverse.
    /// Attacher sur CREDITS (la soucoupe du menu principal).
    /// </summary>
    public class CreditsAnimationController : MonoBehaviour
    {
        [Header("Références")]
        [SerializeField] private RectTransform ufoRectTransform;
        [SerializeField] private GameObject rayGameObject;
        [SerializeField] private CanvasGroup rayCanvasGroup;
        [SerializeField] private CanvasGroup textCanvasGroup;

        [Header("Position ouverte (anchoredPosition cible — diagonale)")]
        [SerializeField] private Vector2 openPosition = new Vector2(-170f, 80f);

        [Header("Rotation ouverte (euler Z en degrés)")]
        [SerializeField] private float openRotationZ = 15f;

        [Header("Durées d'animation")]
        [SerializeField] private float ufoSlideDuration = 0.5f;
        [SerializeField] private float rayFadeDuration = 0.4f;
        [SerializeField] private float textFadeDuration = 0.3f;

        private Vector2 _closedPosition;
        private float _closedRotationZ;
        private bool _isOpen;
        private bool _isAnimating;

        private void Awake()
        {
            _closedPosition = ufoRectTransform.anchoredPosition;
            _closedRotationZ = ufoRectTransform.localEulerAngles.z;

            // État initial : rayon invisible et désactivé, texte invisible
            rayGameObject.SetActive(false);
            rayCanvasGroup.alpha = 0f;
            textCanvasGroup.alpha = 0f;
        }

        /// <summary>Appelé par le Button OnClick de la soucoupe.</summary>
        public void OnUfoClicked()
        {
            if (_isAnimating) return;
            StartCoroutine(_isOpen ? CloseCredits() : OpenCredits());
        }

        private IEnumerator OpenCredits()
        {
            _isAnimating = true;

            // 1. Slide diagonal + rotation simultanés
            yield return SlideAndRotateUfo(_closedPosition, openPosition, _closedRotationZ, openRotationZ, ufoSlideDuration);

            // 2. Le rayon apparaît en fondu
            rayGameObject.SetActive(true);
            yield return Fade(rayCanvasGroup, 0f, 1f, rayFadeDuration);

            // 3. Le texte apparaît en fondu
            yield return Fade(textCanvasGroup, 0f, 1f, textFadeDuration);

            _isOpen = true;
            _isAnimating = false;
        }

        private IEnumerator CloseCredits()
        {
            _isAnimating = true;

            // 1. Le texte disparaît en fondu
            yield return Fade(textCanvasGroup, 1f, 0f, textFadeDuration);

            // 2. Le rayon disparaît en fondu puis se désactive
            yield return Fade(rayCanvasGroup, 1f, 0f, rayFadeDuration);
            rayGameObject.SetActive(false);

            // 3. Slide diagonal + rotation inverses
            yield return SlideAndRotateUfo(openPosition, _closedPosition, openRotationZ, _closedRotationZ, ufoSlideDuration);

            _isOpen = false;
            _isAnimating = false;
        }

        private IEnumerator SlideAndRotateUfo(Vector2 fromPos, Vector2 toPos, float fromRot, float toRot, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                ufoRectTransform.anchoredPosition = Vector2.Lerp(fromPos, toPos, t);
                ufoRectTransform.localEulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(fromRot, toRot, t));
                yield return null;
            }
            ufoRectTransform.anchoredPosition = toPos;
            ufoRectTransform.localEulerAngles = new Vector3(0f, 0f, toRot);
        }

        private IEnumerator Fade(CanvasGroup group, float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / duration));
                group.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }
            group.alpha = to;
        }
    }
}
