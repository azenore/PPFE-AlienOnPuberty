using System;
using System.Collections.Generic;

namespace VN.Save
{
    [Serializable]
    public class SaveData
    {

        // Renseigné si un choix a été sélectionné dans le message currentPhoneMessageIndex
        public int currentPhoneChoiceMessageIndex = -1; // index du message porteur de choix
        public int currentPhoneChoiceIndex = -1;        // index du choix sélectionné dans ce message
        public string protagonistName;
        public float hairColorR, hairColorG, hairColorB;
        public float eyeColorR, eyeColorG, eyeColorB;

        public string currentChapterName;
        public int currentLineIndex;
        public string lastCharacterOnScreenName;

        // Renseigné uniquement si la sauvegarde a lieu pendant un phone chapter
        public string currentPhoneChapterName;
        public int currentPhoneMessageIndex;

        public List<AffinitySaveEntry> affinities = new();
    }

    [Serializable]
    public class AffinitySaveEntry
    {
        public string characterName;
        public int value;
    }
}

