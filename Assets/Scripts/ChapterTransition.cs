using System;
using UnityEngine;

namespace VN.Data
{
    [Serializable]
    public class ChapterTransition
    {
        [Tooltip("Chapitre suivant (dialogue ou téléphone).")]
        public BaseChapter nextChapter;
    }
}
