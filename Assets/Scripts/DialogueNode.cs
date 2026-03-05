using System;
using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class DialogueNode
    {
        public DialogueLine line;
        public bool isChoiceNode;
        public List<DialogueChoice> choices = new();

        [Tooltip("Cocher pour changer l'émotion du protagoniste sur ce node, peu importe qui parle")]
        public bool overrideProtagonistEmotion;
        public EmotionType protagonistEmotion = EmotionType.Neutral;

        [Tooltip("Personnage affiché en grand à l'écran, indépendant de qui parle. Laisser vide pour garder le précédent.")]
        public CharacterData characterOnScreen;
        public EmotionType characterOnScreenEmotion = EmotionType.Neutral;

        [Tooltip("Laisser vide pour garder le background actuel")]
        public Sprite backgroundOverride;
    }
}
