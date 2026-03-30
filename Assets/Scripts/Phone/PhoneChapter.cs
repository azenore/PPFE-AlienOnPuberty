using System.Collections.Generic;
using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Phone Chapter", fileName = "NewPhoneChapter")]
    public class PhoneChapter : BaseChapter
    {
        [Tooltip("Nom du groupe affiché dans l'en-tête. Remplace la liste des participants si renseigné.")]
        public string groupName;

        [Tooltip("Personnages participants (hors protagoniste). Ignoré si groupName est renseigné.")]
        public List<CharacterData> participants = new();

        public List<PhoneMessage> messages = new();

        public ChapterTransition transition;

        /// <summary>Returns the display name to show in the chat header.</summary>
        public string GetHeaderLabel()
        {
            if (!string.IsNullOrWhiteSpace(groupName)) return groupName;
            if (participants == null || participants.Count == 0) return string.Empty;

            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < participants.Count; i++)
            {
                if (participants[i] == null) continue;
                sb.Append(participants[i].characterName);
                if (i < participants.Count - 1) sb.Append(", ");
            }
            return sb.ToString();
        }
    }
}
