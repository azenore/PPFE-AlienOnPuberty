using System;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueLine
    {
        public CharacterData speaker;
        public bool isProtagonist;
        [TextArea(2, 6)] public string text;
        public AudioClip voiceClip;

        public bool IsNarrator => !isProtagonist && speaker == null;
        public bool IsProtagonist => isProtagonist;
        public bool IsCharacter => !isProtagonist && speaker != null;
    }
}
