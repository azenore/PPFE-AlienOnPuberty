using UnityEngine;
using UnityEngine.Audio;
using VN.Settings;

namespace VN.Runtime
{
    public class AudioManager : MonoBehaviour
    {
        private const string MusicVolumeParam = "MusicVolume";
        private const string SFXVolumeParam = "SFXVolume";
        private const float SilenceThreshold = 0.0001f;
        private const float DbMultiplier = 20f;
        private const float MinDb = -80f;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private void Start()
        {
            ApplySettings(SettingsSystem.Load());
        }

        /// <summary>Applies a full SettingsData snapshot to the mixer.</summary>
        public void ApplySettings(SettingsData settings)
        {
            SetMusicVolume(settings.musicVolume);
            SetSFXVolume(settings.sfxVolume);
        }

        /// <summary>Sets music volume from a linear 0-1 value.</summary>
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat(MusicVolumeParam, LinearToDb(volume));
        }

        /// <summary>Sets SFX volume from a linear 0-1 value.</summary>
        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat(SFXVolumeParam, LinearToDb(volume));
        }

        /// <summary>Plays a one-shot SFX clip without interrupting current SFX.</summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip);
        }

        /// <summary>Swaps the music track with a crossfade-ready replacement.</summary>
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        private static float LinearToDb(float volume)
        {
            return volume > SilenceThreshold ? Mathf.Log10(volume) * DbMultiplier : MinDb;
        }
    }
}
