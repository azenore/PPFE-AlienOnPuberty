using System;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueLine
    {
        [Tooltip("Laisser vide si isProtagonist est coché ou si c'est le narrateur")]
        public CharacterData speaker;

        [Tooltip("Cocher si c'est le protagoniste qui parle")]
        public bool isProtagonist;

        public EmotionType speakerEmotion = EmotionType.Neutral;

        [TextArea(2, 6)]
        public string text;

        public AudioClip voiceClip;

        public bool IsNarrator => !isProtagonist && speaker == null;
        public bool IsProtagonist => isProtagonist;
        public bool IsCharacter => !isProtagonist && speaker != null;
    }
}
