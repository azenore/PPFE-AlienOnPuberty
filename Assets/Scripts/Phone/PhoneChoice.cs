using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class PhoneChoice
    {
        [Tooltip("Texte affiché sur le bouton et dans la bulle après sélection.")]
        public string label;

        [Tooltip("Personnage dont l'affinité est modifiée.")]
        public CharacterData affinityTarget;

        public int affinityDelta;

        [Tooltip("Messages révélés après ce choix, dans l'ordre.")]
        public List<PhoneMessage> followUpMessages = new();

        [Tooltip("Chapitre suivant après les follow-ups (dialogue ou téléphone).")]
        public BaseChapter nextChapter;
    }
}
