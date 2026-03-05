// Assets/Scripts/Save/SaveData.cs
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

        public string currentChapterName;  // DialogueChapter.name
        public int currentLineIndex;

        public List<AffinitySaveEntry> affinities = new();
    }

    [Serializable]
    public class AffinitySaveEntry
    {
        public string characterName;  // CharacterData.name
        public int value;
    }
}
