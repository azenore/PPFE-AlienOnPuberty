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

        [Tooltip("Chapitre dialogue suivant par défaut si aucun choix ne le définit.")]
        public DialogueChapter defaultNextChapter;

        [Tooltip("Si défini, ce chapitre téléphone s'active à la fin au lieu du chapitre dialogue.")]
        public PhoneChapter nextPhoneChapter;

        public List<DialogueNode> nodes = new();
    }
}
