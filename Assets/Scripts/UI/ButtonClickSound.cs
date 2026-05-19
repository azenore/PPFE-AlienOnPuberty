using UnityEngine;
using UnityEngine.UI;

namespace VN.UI
{
    /// <summary>Joue un son au clic d'un bouton UI.</summary>
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip clickSound;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlayClickSound);
        }

        private void OnDestroy()
        {
            GetComponent<Button>().onClick.RemoveListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            if (audioSource != null && clickSound != null)
                audioSource.PlayOneShot(clickSound);
        }
    }
}
