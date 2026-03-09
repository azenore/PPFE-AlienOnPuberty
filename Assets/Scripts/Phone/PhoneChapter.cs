using System.Collections.Generic;
using UnityEngine;
using VN.Data;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Phone Chapter", fileName = "NewPhoneChapter")]
    public class PhoneChapter : ScriptableObject
    {
        [Tooltip("Nom du groupe affiché dans l'en-tête. Si renseigné, il remplace la liste des participants.")]
        public string groupName;

        [Tooltip("Personnages participants à la conversation (hors protagoniste). Ignoré si groupName est renseigné.")]
        public List<CharacterData> participants = new();

        public List<PhoneMessage> messages = new();

        [Tooltip("Chapitre dialogue qui suit après cette conversation.")]
        public DialogueChapter defaultNextChapter;

        [Tooltip("Chapitre téléphone qui suit, si la prochaine scène est aussi un téléphone.")]
        public PhoneChapter defaultNextPhoneChapter;

        /// <summary>Returns the display name to show in the chat header.</summary>
        public string GetHeaderLabel()
        {
            if (!string.IsNullOrWhiteSpace(groupName))
                return groupName;

            if (participants == null || participants.Count == 0)
                return string.Empty;

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
