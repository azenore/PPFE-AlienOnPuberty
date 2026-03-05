using System;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueChoice
    {
        [Tooltip("Texte affiché sur le bouton")]
        public string label;

        [Tooltip("Personnage dont l'affinité est modifiée")]
        public CharacterData affinityTarget;

        public int affinityDelta;

        [Tooltip("Chapitre chargé après ce choix")]
        public DialogueChapter nextChapter;
    }
}
