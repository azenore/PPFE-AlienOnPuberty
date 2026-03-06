using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class AffinityBarView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private DialogueEngine engine;

        [Header("UI")]
        [SerializeField] private GameObject barPanel;
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI characterNameText;

        private CharacterData _currentCharacter;

        private void OnEnable()
        {
            engine.OnCharacterOnScreenChanged += HandleCharacterChanged;
            protagonist.OnAffinityChanged += HandleAffinityChanged;
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
            _currentCharacter = character;

            if (_currentCharacter == null)
            {
                barPanel.SetActive(false);
                return;
            }

            barPanel.SetActive(true);
            characterNameText.text = character.characterName;
            fillImage.fillAmount = protagonist.GetAffinity(character) / 100f;
        }

        private void HandleAffinityChanged(CharacterData character, int newValue)
        {
            if (character != _currentCharacter) return;
            fillImage.fillAmount = newValue / 100f;
        }
    }
}
