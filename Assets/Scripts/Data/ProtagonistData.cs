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
        public string hairId;
        public string eyeId;

        [Tooltip("Associe une combinaison hairId/eyeId � des sprites par �motion")]
        [SerializeField] private List<AppearanceEntry> appearances = new();

        [Tooltip("Affinités de départ, identiques à chaque nouvelle partie. Ne jamais modifier en runtime.")]
        [SerializeField] private List<AffinityEntry> baseAffinities = new();

        [SerializeField] private List<AffinityEntry> affinities = new();

        public event Action<CharacterData, int> OnAffinityChanged;
        public event Action<EmotionType> OnEmotionChanged;

        /// <summary>Returns the sprite matching the current hairId, eyeId and the given emotion. Falls back to Neutral.</summary>
        public Sprite GetSprite(EmotionType emotion)
        {
            if (appearances == null) return null;
            foreach (var entry in appearances)
            {
                if (entry.hairId != hairId || entry.eyeId != eyeId) continue;
                return entry.GetSprite(emotion);
            }
            return null;
        }

        /// <summary>Returns a runtime clone of this asset to avoid mutating the original ScriptableObject on disk.</summary>
        public ProtagonistData CreateRuntimeCopy()
        {
            var copy = Instantiate(this);
            copy.name = this.name;
            copy.affinities = new List<AffinityEntry>(affinities);
            return copy;
        }

        /// <summary>Returns current affinity value for a character (0-100).</summary>
        public int GetAffinity(CharacterData character)
        {
            foreach (var entry in affinities)
                if (entry.character == character) return entry.value;
            return 0;
        }

        /// <summary>Returns a snapshot of all base affinity entries (new game defaults).</summary>
        public IEnumerable<(CharacterData character, int value)> GetAllBaseAffinities()
        {
            foreach (var entry in baseAffinities)
                yield return (entry.character, entry.value);
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
        public void ResetAffinities()
        {
            foreach (var entry in affinities)
                OnAffinityChanged?.Invoke(entry.character, 0);
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
        public class AppearanceEntry
        {
            public string hairId;
            public string eyeId;
            // Pas de = new() ici � �vite l'appel depuis le loading thread
            [SerializeField] private List<EmotionSprite> emotionSprites;

            /// <summary>Returns the sprite for the given emotion, falls back to Neutral.</summary>
            public Sprite GetSprite(EmotionType emotion)
            {
                if (emotionSprites == null) return null;
                Sprite fallback = null;
                foreach (var es in emotionSprites)
                {
                    if (es.emotion == emotion) return es.sprite;
                    if (es.emotion == EmotionType.Neutral) fallback = es.sprite;
                }
                return fallback;
            }
        }

        [Serializable]
        private struct EmotionSprite
        {
            public EmotionType emotion;
            public Sprite sprite;
        }

        [Serializable]
        private struct AffinityEntry
        {
            public CharacterData character;
            public int value;
        }
    }
}
