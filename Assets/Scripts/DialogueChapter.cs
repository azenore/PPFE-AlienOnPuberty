using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Chapter", fileName = "NewChapter")]
    public class DialogueChapter : BaseChapter
    {
        public string chapterTitle;
        public Sprite background;
        public AudioClip backgroundMusic;

        [Tooltip("Si renseigné, remplace 'background' par le sprite correspondant aux attributs du protagoniste.")]
        public ConditionalBackground conditionalBackground;

        public List<DialogueNode> nodes = new();

        public ChapterTransition transition;

        [Tooltip("Si une condition est remplie, son chapitre est charg� � la place de la transition par d�faut.")]
        public List<AffinityRequirement> affinityRequirements = new();

        /// <summary>Returns the unlocked chapter if any affinity requirement is met, null otherwise.</summary>
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
        public CharacterData character;
        [Range(0, 100)] public int minimumAffinity;
        public DialogueChapter unlockedChapter;

        /// <summary>Returns true if the protagonist's affinity meets the minimum.</summary>
        public bool IsMet(ProtagonistData protagonist)
            => protagonist.GetAffinity(character) >= minimumAffinity;
    }
}
