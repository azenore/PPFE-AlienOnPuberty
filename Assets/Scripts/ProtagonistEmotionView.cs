using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class ProtagonistEmotionView : MonoBehaviour
    {
        [Serializable]
        private struct EmotionSprite
        {
            public EmotionType emotion;
            public Sprite sprite;
        }

        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private Image emotionImage;
        [SerializeField] private List<EmotionSprite> emotionSprites;

        private Dictionary<EmotionType, Sprite> _cache;

        private void Awake()
        {
            _cache = new Dictionary<EmotionType, Sprite>(emotionSprites.Count);
            foreach (var entry in emotionSprites)
                _cache.TryAdd(entry.emotion, entry.sprite);
        }

        private void Start()
        {
            // Affiche l'émotion courante dès l'activation (Neutral par défaut)
            UpdateEmotion(protagonist.currentEmotion);
        }

        private void OnEnable() => protagonist.OnEmotionChanged += UpdateEmotion;
        private void OnDisable() => protagonist.OnEmotionChanged -= UpdateEmotion;

        private void UpdateEmotion(EmotionType emotion)
        {
            if (_cache == null) return;

            if (_cache.TryGetValue(emotion, out Sprite sprite))
                emotionImage.sprite = sprite;
            else if (_cache.TryGetValue(EmotionType.Neutral, out Sprite neutral))
                emotionImage.sprite = neutral;
        }
    }
}
