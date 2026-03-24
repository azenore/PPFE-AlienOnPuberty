using System;
using System.Collections.Generic;

namespace VN.Save
{
    [Serializable]
    public class SaveData
    {
        public int currentPhoneChoiceMessageIndex = -1; 
        public int currentPhoneChoiceIndex = -1;       
        public string protagonistName;
        public float hairColorR, hairColorG, hairColorB;
        public float eyeColorR, eyeColorG, eyeColorB;
        public string hairId; 
        public string eyeId;

        public string currentChapterName;
        public int currentLineIndex;
        public string lastCharacterOnScreenName;

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

