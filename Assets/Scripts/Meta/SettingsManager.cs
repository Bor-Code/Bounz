

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

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave.settings != null)

        {

            CurrentSettings = SaveManager.Instance.CurrentSave.settings;

        }

        else

        {

            string json = SaveManager.GetStringValue(SettingsSaveKey, "");

            CurrentSettings = string.IsNullOrEmpty(json) ? new SettingsData() : JsonUtility.FromJson<SettingsData>(json);

        }

        if (CurrentSettings == null) CurrentSettings = new SettingsData();

        PersistToSaveManager();

        ApplySettings();

    }

    public void SaveSettings()

    {

        PersistToSaveManager();

        ApplySettings();

    }

    public void ApplySettings()

    {

        if (CurrentSettings == null) CurrentSettings = new SettingsData();

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

    private void PersistToSaveManager()

    {

        if (SaveManager.Instance != null)

        {

            SaveManager.Instance.CurrentSave.settings = CurrentSettings;

            SaveManager.Instance.SaveGame();

        }

        else

        {

            SaveManager.SetStringValue(SettingsSaveKey, JsonUtility.ToJson(CurrentSettings));

        }

    }

}

