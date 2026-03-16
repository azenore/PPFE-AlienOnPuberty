using UnityEngine;
using UnityEngine.UI;
using VN.Data;
using VN.Runtime;

namespace VN.UI
{
    public class ProtagonistEmotionView : MonoBehaviour
    {
        [SerializeField] private ProtagonistData protagonist;
        [SerializeField] private DialogueEngine dialogueEngine;
        [SerializeField] private Image emotionImage;

        private EmotionType _currentEmotion = EmotionType.Neutral;

        private void Start()
        {
            Refresh();
        }

        private void OnEnable()
        {
            dialogueEngine.OnProtagonistEmotionChanged += OnEmotionChanged;
            dialogueEngine.OnLineReady += OnLineReady;
        }

        private void OnDisable()
        {
            dialogueEngine.OnProtagonistEmotionChanged -= OnEmotionChanged;
            dialogueEngine.OnLineReady -= OnLineReady;
        }

        private void OnEmotionChanged(EmotionType emotion)
        {
            _currentEmotion = emotion;
            Refresh();
        }

        private void OnLineReady(DialogueLine _)
        {
            Refresh();
        }

        private void Refresh()
        {
            Sprite sprite = protagonist.GetSprite(_currentEmotion);
            if (sprite != null)
            {
                emotionImage.sprite = sprite;
                emotionImage.gameObject.SetActive(true);
            }
        }
    }
}
