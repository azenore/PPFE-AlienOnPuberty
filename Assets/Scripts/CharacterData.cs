using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Character", fileName = "NewCharacter")]
    public class CharacterData : ScriptableObject
    {
        public string characterName;
        public Color nameColor = Color.white;

        [SerializeField]
        private List<EmotionEntry> emotionSprites = new();

        // Cache construit une seule fois pour éviter les boucles foreach à chaque ligne
        private Dictionary<EmotionType, Sprite> _spriteCache;

        private void OnEnable() => BuildCache();

        private void BuildCache()
        {
            _spriteCache = new Dictionary<EmotionType, Sprite>(emotionSprites.Count);
            foreach (var entry in emotionSprites)
                _spriteCache.TryAdd(entry.emotion, entry.sprite);
        }

        /// <summary>Returns the sprite matching the given emotion, falls back to Neutral.</summary>
        public Sprite GetSprite(EmotionType emotion)
        {
            if (_spriteCache == null) BuildCache();

            if (_spriteCache.TryGetValue(emotion, out Sprite sprite)) return sprite;
            if (_spriteCache.TryGetValue(EmotionType.Neutral, out Sprite neutral)) return neutral;
            return null;
        }

        [Serializable]
        private struct EmotionEntry
        {
            public EmotionType emotion;
            public Sprite sprite;
        }
    }
}
