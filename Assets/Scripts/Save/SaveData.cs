using System;
using System.Collections.Generic;

namespace VN.Save
{
    [Serializable]
    public class SaveData
    {
        public string protagonistName;
        public float hairColorR, hairColorG, hairColorB;
        public float eyeColorR, eyeColorG, eyeColorB;

        public string currentChapterName;
        public int currentLineIndex;
        public string lastCharacterOnScreenName;

        public List<AffinitySaveEntry> affinities = new();
    }

    [Serializable]
    public class AffinitySaveEntry
    {
        public string characterName;
        public int value;
    }
}
