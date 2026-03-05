using System;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueLine
    {
        [Tooltip("Laisser vide pour le narrateur")]
        public CharacterData speaker;

        public EmotionType speakerEmotion = EmotionType.Neutral;

        [TextArea(2, 6)]
        public string text;

        public AudioClip voiceClip;

        public bool IsNarrator => speaker == null;
    }
}
