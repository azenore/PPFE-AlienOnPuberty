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

        private void OnEnable() => engine.OnCharacterOnScreenChanged += UpdateSprite;
        private void OnDisable() => engine.OnCharacterOnScreenChanged -= UpdateSprite;

        private void UpdateSprite(CharacterData character, EmotionType emotion)
        {
            Sprite sprite = character.GetSprite(emotion);
            characterSpriteImage.sprite = sprite;
            characterSpriteImage.gameObject.SetActive(sprite != null);
        }
    }
}
