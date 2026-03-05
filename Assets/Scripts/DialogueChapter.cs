using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Chapter", fileName = "NewChapter")]
    public class DialogueChapter : ScriptableObject
    {
        public string chapterTitle;
        public Sprite background;
        public AudioClip backgroundMusic;

        [Tooltip("Chapitre suivant par défaut si aucun choix ne le définit")]
        public DialogueChapter defaultNextChapter;

        public List<DialogueNode> nodes = new();
    }
}

