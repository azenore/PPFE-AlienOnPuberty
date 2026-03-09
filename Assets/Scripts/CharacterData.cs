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

        // Cache construit à la demande sur le main thread uniquement
        private Dictionary<EmotionType, Sprite> _spriteCache;

        // OnEnable() retiré — il était appelé depuis le loading thread lors du chargement de scène,
        // ce qui provoquait des erreurs de thread et de concurrence sur le Dictionary.

        private void BuildCache()
        {
            // Toujours recréer le dictionnaire pour éviter les états corrompus entre hot reloads
            _spriteCache = new Dictionary<EmotionType, Sprite>(emotionSprites.Count);
            foreach (var entry in emotionSprites)
                _spriteCache.TryAdd(entry.emotion, entry.sprite);
        }

        /// <summary>Returns the sprite matching the given emotion, falls back to Neutral.</summary>
        public Sprite GetSprite(EmotionType emotion)
        {
            // Lazy init : construit le cache uniquement au premier appel, garanti sur le main thread
            if (_spriteCache == null) BuildCache();

            if (_spriteCache.TryGetValue(emotion, out Sprite sprite)) return sprite;
            if (_spriteCache.TryGetValue(EmotionType.Neutral, out Sprite neutral)) return neutral;
            return null;
        }

        /// <summary>Forces a cache rebuild, e.g. after modifying emotionSprites at runtime.</summary>
        public void InvalidateCache()
        {
            _spriteCache = null;
        }

        [Serializable]
        private struct EmotionEntry
        {
            public EmotionType emotion;
            public Sprite sprite;
        }
    }
}
