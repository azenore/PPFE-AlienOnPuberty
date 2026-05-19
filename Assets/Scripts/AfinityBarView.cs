using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class AffinityBarView : MonoBehaviour
    {
        private const float TransitionDuration = 0.6f;

        [Header("References")]
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private DialogueEngine engine;

        [Header("UI")]
        [SerializeField] private GameObject barPanel;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI characterNameText;

        private CharacterData _currentCharacter;
        private Coroutine _fillCoroutine;

        private void OnEnable()
        {
            engine.OnCharacterOnScreenChanged += HandleCharacterChanged;
            protagonist.OnAffinityChanged += HandleAffinityChanged;
            ForceRefresh();
        }

        private void OnDisable()
        {
            engine.OnCharacterOnScreenChanged -= HandleCharacterChanged;
            protagonist.OnAffinityChanged -= HandleAffinityChanged;
        }

        /// <summary>Appelé explicitement après LoadGame() pour forcer le rafraîchissement de la barre.</summary>
        public void ForceRefresh()
        {
            if (engine.CurrentCharacter != null)
                HandleCharacterChanged(engine.CurrentCharacter, EmotionType.Neutral);
            else
                barPanel.SetActive(false);
        }

        private void HandleCharacterChanged(CharacterData character, EmotionType _)
        {
            bool sameCharacter = character == _currentCharacter;
            _currentCharacter = character;

            if (_currentCharacter == null)
            {
                barPanel.SetActive(false);
                return;
            }

            barPanel.SetActive(true);
            characterNameText.text = character.characterName;

            if (!sameCharacter)
            {
                StopFillCoroutine();
                fillImage.fillAmount = protagonist.GetAffinity(character) / 100f;
            }
        }

        private void HandleAffinityChanged(CharacterData character, int newValue)
        {
            if (character != _currentCharacter) return;
            AnimateFill(newValue / 100f);
        }

        private void AnimateFill(float targetFill)
        {
            StopFillCoroutine();
            _fillCoroutine = StartCoroutine(FillRoutine(targetFill));
        }

        private void StopFillCoroutine()
        {
            if (_fillCoroutine != null)
            {
                StopCoroutine(_fillCoroutine);
                _fillCoroutine = null;
            }
        }

        private IEnumerator FillRoutine(float targetFill)
        {
            float startFill = fillImage.fillAmount;
            float elapsed = 0f;

            while (elapsed < TransitionDuration)
            {
                elapsed += Time.deltaTime;
                fillImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / TransitionDuration);
                yield return null;
            }

            fillImage.fillAmount = targetFill;
            _fillCoroutine = null;
        }
    }
}
