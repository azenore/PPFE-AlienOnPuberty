using UnityEngine;
using UnityEngine.UI;
using VN.Runtime;
using VN.Settings;

namespace VN.UI
{
    public class OptionsController : MonoBehaviour
    {
        [Header("Sliders")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("References")]
        [SerializeField] private AudioManager audioManager;

        private SettingsData _currentSettings;

        private void OnEnable()
        {
            _currentSettings = SettingsSystem.Load();

            // Initialise les sliders sans déclencher les callbacks
            musicSlider.SetValueWithoutNotify(_currentSettings.musicVolume);
            sfxSlider.SetValueWithoutNotify(_currentSettings.sfxVolume);

            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }

        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
        }

        private void OnMusicChanged(float value)
        {
            _currentSettings.musicVolume = value;
            audioManager.SetMusicVolume(value);
            SettingsSystem.Save(_currentSettings);
        }

        private void OnSFXChanged(float value)
        {
            _currentSettings.sfxVolume = value;
            audioManager.SetSFXVolume(value);
            SettingsSystem.Save(_currentSettings);
        }
    }
}
