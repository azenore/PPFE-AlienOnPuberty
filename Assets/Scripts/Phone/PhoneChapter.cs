using System.Collections.Generic;
using UnityEngine;
using VN.Data;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Phone Chapter", fileName = "NewPhoneChapter")]
    public class PhoneChapter : ScriptableObject
    {
        [Tooltip("Personnages participants à la conversation (hors protagoniste).")]
        public List<CharacterData> participants = new();

        public List<PhoneMessage> messages = new();

        [Tooltip("Chapitre dialogue qui suit après cette conversation.")]
        public DialogueChapter defaultNextChapter;

        [Tooltip("Chapitre téléphone qui suit, si la prochaine scène est aussi un téléphone.")]
        public PhoneChapter defaultNextPhoneChapter;
    }
}
