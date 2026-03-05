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

        /// <summary>Returns the sprite matching the given emotion, falls back to Neutral.</summary>
        public Sprite GetSprite(EmotionType emotion)
        {
            foreach (var entry in emotionSprites)
                if (entry.emotion == emotion) return entry.sprite;

            foreach (var entry in emotionSprites)
                if (entry.emotion == EmotionType.Neutral) return entry.sprite;

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

