using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueNode
    {
        [Header("Dialogue")]
        [Tooltip("Laisser vide pour que le protagoniste parle.")]
        public CharacterData speaker;
        [TextArea(2, 6)] public string text;
        public AudioClip voiceClip;

        [Header("Choix")]
        public List<DialogueChoice> choices = new();

        [Header("Personnage à l'écran")]
        [Tooltip("Laisser vide pour garder le personnage précédent.")]
        public CharacterData characterOnScreen;
        public EmotionType characterOnScreenEmotion = EmotionType.Neutral;

        [Header("Protagoniste")]
        public bool overrideProtagonistEmotion;
        public EmotionType protagonistEmotion = EmotionType.Neutral;

        [Header("Divers")]
        [Tooltip("Cache le personnage à l'écran et la barre d'affinité.")]
        public bool isMonologue;
        public Sprite backgroundOverride;

        public bool IsChoiceNode => choices != null && choices.Count > 0;
    }
}
