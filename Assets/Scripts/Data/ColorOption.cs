using UnityEngine;

namespace VN.Data
{
    [CreateAssetMenu(menuName = "VN/Color Option", fileName = "NewColorOption")]
    public class ColorOption : ScriptableObject
    {
        public string optionName;
        public string id;
        public Color color = Color.white;
    }
}
