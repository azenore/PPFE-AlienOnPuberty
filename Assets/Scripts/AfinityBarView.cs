using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class AffinityBarView : MonoBehaviour
    {
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private CharacterData trackedCharacter;
        [SerializeField] private Image fillImage; // Image en mode "Filled", Fill Method = Vertical

        private void OnEnable() => protagonist.OnAffinityChanged += HandleAffinityChanged;
        private void OnDisable() => protagonist.OnAffinityChanged -= HandleAffinityChanged;

        private void Start()
        {
            // Initialise la barre au démarrage
            float initial = protagonist.GetAffinity(trackedCharacter) / 100f;
            fillImage.fillAmount = initial;
        }

        private void HandleAffinityChanged(CharacterData character, int newValue)
        {
            if (character != trackedCharacter) return;
            fillImage.fillAmount = newValue / 100f;
        }
    }
}
