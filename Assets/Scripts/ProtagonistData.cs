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

        /// <summary>Returns a snapshot of all affinity entries for serialization.</summary>
        public IEnumerable<(CharacterData character, int value)> GetAllAffinities()
        {
            foreach (var entry in affinities)
                yield return (entry.character, entry.value);
        }

        /// <summary>Sets affinity to an absolute value, clamped 0-100. Used when loading a save.</summary>
        public void SetAffinity(CharacterData character, int value)
        {
            int clamped = Mathf.Clamp(value, 0, 100);
            for (int i = 0; i < affinities.Count; i++)
            {
                if (affinities[i].character != character) continue;
                var entry = affinities[i];
                entry.value = clamped;
                affinities[i] = entry;
                OnAffinityChanged?.Invoke(character, clamped);
                return;
            }
            affinities.Add(new AffinityEntry { character = character, value = clamped });
            OnAffinityChanged?.Invoke(character, clamped);
        }

        /// <summary>Clears all affinity entries. Used before restoring from a save.</summary>
        public void ResetAffinities()
        {
            affinities.Clear();
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
