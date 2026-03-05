using System;
using System.Collections.Generic;

namespace VN.Data
{
    [Serializable]
    public class DialogueNode
    {
        public DialogueLine line;
        public bool isChoiceNode;
        public List<DialogueChoice> choices = new();
    }
}
