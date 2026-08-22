using UnityEngine;
using System;
[Serializable]
public class SettingsData
{
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool hapticsEnabled = true;
}
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }
    private const string SettingsSaveKey = "GameSettings";
    public SettingsData CurrentSettings { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSettings();
    }
    public void LoadSettings()
    {
        string json = PlayerPrefs.GetString(SettingsSaveKey, "");
        if (string.IsNullOrEmpty(json))
        {
            CurrentSettings = new SettingsData();
        }
        else
        {
            CurrentSettings = JsonUtility.FromJson<SettingsData>(json);
        }
        ApplySettings();
    }
    public void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings);
        PlayerPrefs.SetString(SettingsSaveKey, json);
        PlayerPrefs.Save();
        ApplySettings();
    }
    private void ApplySettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(CurrentSettings.musicVolume);
            AudioManager.Instance.SetSFXVolume(CurrentSettings.sfxVolume);
        }
        if (HapticManager.Instance != null)
        {
            HapticManager.Instance.HapticsEnabled = CurrentSettings.hapticsEnabled;
        }
    }
    public void SetMusicVolume(float volume)
    {
        CurrentSettings.musicVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    public void SetSFXVolume(float volume)
    {
        CurrentSettings.sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
    }
    public void ToggleHaptics(bool isEnabled)
    {
        CurrentSettings.hapticsEnabled = isEnabled;
        SaveSettings();
    }
}
