using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Protagonist", fileName = "ProtagonistData")]
    public class ProtagonistData : ScriptableObject
    {
        public string playerName = "Yuki";
        public EmotionType currentEmotion = EmotionType.Neutral;
        public Color hairColor = Color.white;
        public Color eyeColor = Color.white;

        [Tooltip("CharacterData contenant les sprites du protagoniste")]
        public CharacterData characterData;

        [SerializeField]
        private List<AffinityEntry> affinities = new();

        public event Action<CharacterData, int> OnAffinityChanged;
        public event Action<EmotionType> OnEmotionChanged;

        /// <summary>Returns current affinity value for a character (0-100).</summary>
        public int GetAffinity(CharacterData character)
        {
            foreach (var entry in affinities)
                if (entry.character == character) return entry.value;
            return 0;
        }

        /// <summary>Adds delta to affinity, clamped between 0 and 100.</summary>
        public void ModifyAffinity(CharacterData character, int delta)
        {
            for (int i = 0; i < affinities.Count; i++)
            {
                if (affinities[i].character != character) continue;

                var entry = affinities[i];
                entry.value = Mathf.Clamp(entry.value + delta, 0, 100);
                affinities[i] = entry;
                OnAffinityChanged?.Invoke(character, entry.value);
                return;
            }

            int newValue = Mathf.Clamp(delta, 0, 100);
            affinities.Add(new AffinityEntry { character = character, value = newValue });
            OnAffinityChanged?.Invoke(character, newValue);
        }

        /// <summary>Sets the protagonist's current emotion.</summary>
        public void SetEmotion(EmotionType emotion)
        {
            currentEmotion = emotion;
            OnEmotionChanged?.Invoke(emotion);
        }

        [Serializable]
        private struct AffinityEntry
        {
            public CharacterData character;
            public int value;
        }
    }
}
