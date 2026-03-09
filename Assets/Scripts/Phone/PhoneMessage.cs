using System;
using UnityEngine;
using VN.Data;

namespace VN.Data
{
    [Serializable]
    public class PhoneMessage
    {
        [Tooltip("Laisser vide = le protagoniste envoie ce message.")]
        public CharacterData sender;

        [TextArea(2, 5)]
        public string text;

        public bool IsFromProtagonist => sender == null;
    }
}
