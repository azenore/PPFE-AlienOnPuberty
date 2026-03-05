using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class ProtagonistEmotionView : MonoBehaviour
    {
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private Image emotionImage;

        [SerializeField] private Sprite neutralSprite;
        [SerializeField] private Sprite happySprite;
        [SerializeField] private Sprite sadSprite;
        [SerializeField] private Sprite angrySprite;
        [SerializeField] private Sprite surprisedSprite;
        [SerializeField] private Sprite embarrassedSprite;
        [SerializeField] private Sprite shySprite;

        private void OnEnable() => protagonist.OnEmotionChanged += UpdateEmotion;
        private void OnDisable() => protagonist.OnEmotionChanged -= UpdateEmotion;

        private void UpdateEmotion(EmotionType emotion)
        {
            emotionImage.sprite = emotion switch
            {
                EmotionType.Happy => happySprite,
                EmotionType.Sad => sadSprite,
                EmotionType.Angry => angrySprite,
                EmotionType.Surprised => surprisedSprite,
                EmotionType.Embarrassed => embarrassedSprite,
                EmotionType.Shy => shySprite,
                _ => neutralSprite
            };
        }
    }
}
