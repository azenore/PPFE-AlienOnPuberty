using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class CharacterSpriteController : MonoBehaviour
    {
        [SerializeField] private DialogueEngine engine;
        [SerializeField] private Image characterSpriteImage;

        [Tooltip("Barre d'affinité à cacher pendant un monologue.")]
        [SerializeField] private GameObject affinityBarPanel;

        private void OnEnable()
        {
            engine.OnCharacterOnScreenChanged += UpdateSprite;
            engine.OnMonologueStateChanged += OnMonologueStateChanged;
        }

        private void OnDisable()
        {
            engine.OnCharacterOnScreenChanged -= UpdateSprite;
            engine.OnMonologueStateChanged -= OnMonologueStateChanged;
        }

        private void UpdateSprite(CharacterData character, EmotionType emotion)
        {
            Sprite sprite = character.GetSprite(emotion);
            characterSpriteImage.sprite = sprite;
            characterSpriteImage.gameObject.SetActive(sprite != null);
        }

        private void OnMonologueStateChanged(bool isMonologue)
        {
            characterSpriteImage.gameObject.SetActive(!isMonologue);
            if (affinityBarPanel != null)
                affinityBarPanel.SetActive(!isMonologue);
        }
    }
}
