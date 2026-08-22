using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle hapticsToggle;

    private void Start()
    {
        if (SettingsManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = SettingsManager.Instance.CurrentSettings.musicVolume;
                musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = SettingsManager.Instance.CurrentSettings.sfxVolume;
                sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
            if (hapticsToggle != null)
            {
                hapticsToggle.isOn = SettingsManager.Instance.CurrentSettings.hapticsEnabled;
                hapticsToggle.onValueChanged.AddListener(OnHapticsToggled);
            }
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.SetSFXVolume(value);
    }

    private void OnHapticsToggled(bool value)
    {
        if (SettingsManager.Instance != null)
            SettingsManager.Instance.ToggleHaptics(value);
    }
}
