using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VN.Data;

namespace VN.UI
{
    public class MessageBubbleView : MonoBehaviour
    {
        private const string NamePlaceholder = "[NAME]";
        private const float AnimDuration = 0.18f;
        private const int SpriteSize = 64;
        private const int CornerRadius = 20;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI senderNameText;
        [SerializeField] private Image bubbleBackground;
        [SerializeField] private RectTransform inner;

        [Header("Largeur max bulle (px)")]
        [SerializeField] private float maxBubbleWidth = 320f;

        [Header("Couleurs")]
        [SerializeField] private Color protagonistColor = new Color(0.33f, 0.64f, 1f);
        [SerializeField] private Color characterColor = new Color(0.92f, 0.92f, 0.92f);
        [SerializeField] private Color protagonistTextColor = Color.white;
        [SerializeField] private Color characterTextColor = Color.black;

        private static Sprite _roundedSprite;

        private HorizontalLayoutGroup _hlg;
        private LayoutElement _innerLayout;
        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            _hlg = GetComponent<HorizontalLayoutGroup>();
            _innerLayout = inner.GetComponent<LayoutElement>()
                        ?? inner.gameObject.AddComponent<LayoutElement>();

            _rt.pivot = new Vector2(0.5f, 0f);
            transform.localScale = new Vector3(1f, 0f, 1f);

            bubbleBackground.sprite = GetRoundedSprite();
            bubbleBackground.type = Image.Type.Sliced;
        }

        /// <summary>Configures the bubble with animation. Use for new incoming messages.</summary>
        public void Setup(PhoneMessage message, string protagonistName)
        {
            ApplyContent(message, protagonistName);
            StartCoroutine(AppearAnimation());
        }

        /// <summary>Configures the bubble instantly without animation. Use when replaying history on load.</summary>
        public void SetupInstant(PhoneMessage message, string protagonistName)
        {
            ApplyContent(message, protagonistName);
            transform.localScale = Vector3.one;
        }

        private void OnDisable()
        {
            transform.localScale = Vector3.one;
        }

        // Shared content assignment between Setup and SetupInstant
        private void ApplyContent(PhoneMessage message, string protagonistName)
        {
            messageText.text = message.text.Replace(NamePlaceholder, protagonistName);

            _innerLayout.preferredWidth = -1;
            _innerLayout.flexibleWidth = 0;
            _innerLayout.minWidth = 80f;

            _hlg.childForceExpandWidth = false;
            _hlg.childForceExpandHeight = false;

            if (message.IsFromProtagonist)
            {
                senderNameText.text = protagonistName;
                bubbleBackground.color = protagonistColor;
                messageText.color = protagonistTextColor;
                senderNameText.color = protagonistTextColor;
                _hlg.childAlignment = TextAnchor.UpperRight;
            }
            else
            {
                senderNameText.text = message.sender.characterName;
                bubbleBackground.color = characterColor;
                messageText.color = characterTextColor;
                senderNameText.color = characterTextColor;
                _hlg.childAlignment = TextAnchor.UpperLeft;
            }
        }

        private IEnumerator AppearAnimation()
        {
            yield return null;

            if (inner.rect.width > maxBubbleWidth)
                _innerLayout.preferredWidth = maxBubbleWidth;

            float elapsed = 0f;

            while (elapsed < AnimDuration)
            {
                elapsed += Time.deltaTime;
                float ease = EaseOutCubic(Mathf.Clamp01(elapsed / AnimDuration));
                transform.localScale = new Vector3(1f, ease, 1f);
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        private static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

        private static Sprite GetRoundedSprite()
        {
            if (_roundedSprite != null) return _roundedSprite;

            Texture2D tex = new Texture2D(SpriteSize, SpriteSize, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;

            Color[] pixels = new Color[SpriteSize * SpriteSize];

            for (int y = 0; y < SpriteSize; y++)
                for (int x = 0; x < SpriteSize; x++)
                    pixels[y * SpriteSize + x] = IsInsideRoundedRect(x, y) ? Color.white : Color.clear;

            tex.SetPixels(pixels);
            tex.Apply();

            _roundedSprite = Sprite.Create(
                tex,
                new Rect(0, 0, SpriteSize, SpriteSize),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(CornerRadius, CornerRadius, CornerRadius, CornerRadius)
            );

            return _roundedSprite;
        }

        private static bool IsInsideRoundedRect(int x, int y)
        {
            int r = CornerRadius;
            int s = SpriteSize;

            if (x < r && y < r)
                return Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r)) <= r;
            if (x < r && y >= s - r)
                return Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, s - r)) <= r;
            if (x >= s - r && y < r)
                return Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(s - r, r)) <= r;
            if (x >= s - r && y >= s - r)
                return Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(s - r, s - r)) <= r;

            return true;
        }
    }
}
