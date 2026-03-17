using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Chapter", fileName = "NewChapter")]
    public class DialogueChapter : ScriptableObject
    {
        public string chapterTitle;
        public Sprite background;
        public AudioClip backgroundMusic;

        [Tooltip("Chapitre dialogue suivant par défaut si aucune condition d'affinité n'est remplie.")]
        public DialogueChapter defaultNextChapter;

        [Tooltip("Si défini, ce chapitre téléphone s'active à la fin au lieu du chapitre dialogue.")]
        public PhoneChapter nextPhoneChapter;

        [Tooltip("Si une condition est remplie, son chapitre est chargé à la place du suivant par défaut.")]
        public List<AffinityRequirement> affinityRequirements = new();

        public List<DialogueNode> nodes = new();

        /// <summary>Returns the unlocked chapter if any requirement is met, null otherwise.</summary>
        public DialogueChapter GetUnlockedChapter(ProtagonistData protagonist)
        {
            foreach (var req in affinityRequirements)
                if (req.IsMet(protagonist)) return req.unlockedChapter;
            return null;
        }
    }

    [Serializable]
    public class AffinityRequirement
    {
        [Tooltip("Le personnage concerné.")]
        public CharacterData character;

        [Tooltip("Valeur minimale d'affinité requise (0-100).")]
        [Range(0, 100)] public int minimumAffinity;

        [Tooltip("Chapitre chargé si la condition est remplie.")]
        public DialogueChapter unlockedChapter;

        /// <summary>Returns true if the protagonist's affinity meets the minimum.</summary>
        public bool IsMet(ProtagonistData protagonist)
            => protagonist.GetAffinity(character) >= minimumAffinity;
    }
}
