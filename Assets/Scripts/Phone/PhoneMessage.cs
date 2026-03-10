using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class PhoneMessage
    {
        [Tooltip("Laisser vide = le protagoniste envoie ce message.")]
        public CharacterData sender;

        [TextArea(2, 5)]
        public string text;

        [Tooltip("Si renseigné, un menu de choix s'affiche après ce message.")]
        public List<PhoneChoice> choices = new();

        public bool IsFromProtagonist => sender == null;

        /// <summary>True if this message triggers a choice menu.</summary>
        public bool HasChoices => choices != null && choices.Count > 0;
    }
}
