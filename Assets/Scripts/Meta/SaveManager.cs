
using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class IntSaveEntry
{
    public string key;
    public int value;
}

[Serializable]
public class StringSaveEntry
{
    public string key;
    public string value;
}

[Serializable]
public class GameSaveData
{
    public int saveVersion = 2;
    public int highScore;
    public int totalScore;
    public string selectedSkinId;
    public List<string> unlockedSkins = new List<string>();
    public List<string> unlockedAchievements = new List<string>();
    public string questDataJson;
    public SettingsData settings = new SettingsData();
    public List<IntSaveEntry> intValues = new List<IntSaveEntry>();
    public List<StringSaveEntry> stringValues = new List<StringSaveEntry>();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "BounzGameSave";
    private const string LegacyTotalScoreKey = "TotalScore";
    private const string LegacyHighScoreKey = "HighScore";
    private const string LegacySelectedSkinKey = "SelectedSkin";

    public GameSaveData CurrentSave { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadGame();
    }

    public void LoadGame()
    {
        string json = PlayerPrefs.GetString(SaveKey, "");
        if (string.IsNullOrEmpty(json))
        {
            CurrentSave = new GameSaveData();
        }
        else
        {
            CurrentSave = JsonUtility.FromJson<GameSaveData>(json) ?? new GameSaveData();
        }

        EnsureCollections();
        MigrateLegacyPlayerPrefs();
    }

    public void SaveGame()
    {
        EnsureCollections();
        string json = JsonUtility.ToJson(CurrentSave);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public int GetInt(string key, int defaultValue = 0)
    {
        EnsureCollections();
        IntSaveEntry entry = CurrentSave.intValues.Find(x => x.key == key);
        if (entry != null) return entry.value;

        if (PlayerPrefs.HasKey(key))
        {
            int legacyValue = PlayerPrefs.GetInt(key, defaultValue);
            SetInt(key, legacyValue);
            return legacyValue;
        }

        return defaultValue;
    }

    public void SetInt(string key, int value)
    {
        EnsureCollections();
        IntSaveEntry entry = CurrentSave.intValues.Find(x => x.key == key);
        if (entry == null)
        {
            entry = new IntSaveEntry { key = key, value = value };
            CurrentSave.intValues.Add(entry);
        }
        else
        {
            entry.value = value;
        }
        SaveGame();
    }

    public string GetString(string key, string defaultValue = "")
    {
        EnsureCollections();
        StringSaveEntry entry = CurrentSave.stringValues.Find(x => x.key == key);
        if (entry != null) return entry.value;

        if (PlayerPrefs.HasKey(key))
        {
            string legacyValue = PlayerPrefs.GetString(key, defaultValue);
            SetString(key, legacyValue);
            return legacyValue;
        }

        return defaultValue;
    }

    public void SetString(string key, string value)
    {
        EnsureCollections();
        StringSaveEntry entry = CurrentSave.stringValues.Find(x => x.key == key);
        if (entry == null)
        {
            entry = new StringSaveEntry { key = key, value = value };
            CurrentSave.stringValues.Add(entry);
        }
        else
        {
            entry.value = value;
        }
        SaveGame();
    }

    public static int GetIntValue(string key, int defaultValue = 0)
    {
        return Instance != null ? Instance.GetInt(key, defaultValue) : PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetIntValue(string key, int value)
    {
        if (Instance != null)
        {
            Instance.SetInt(key, value);
        }
        else
        {
            PlayerPrefs.SetInt(key, value);
            PlayerPrefs.Save();
        }
    }

    public static string GetStringValue(string key, string defaultValue = "")
    {
        return Instance != null ? Instance.GetString(key, defaultValue) : PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetStringValue(string key, string value)
    {
        if (Instance != null)
        {
            Instance.SetString(key, value);
        }
        else
        {
            PlayerPrefs.SetString(key, value);
            PlayerPrefs.Save();
        }
    }

    private void EnsureCollections()
    {
        if (CurrentSave == null) CurrentSave = new GameSaveData();
        if (CurrentSave.unlockedSkins == null) CurrentSave.unlockedSkins = new List<string>();
        if (CurrentSave.unlockedAchievements == null) CurrentSave.unlockedAchievements = new List<string>();
        if (CurrentSave.settings == null) CurrentSave.settings = new SettingsData();
        if (CurrentSave.intValues == null) CurrentSave.intValues = new List<IntSaveEntry>();
        if (CurrentSave.stringValues == null) CurrentSave.stringValues = new List<StringSaveEntry>();
    }

    private void MigrateLegacyPlayerPrefs()
    {
        bool changed = false;

        if (PlayerPrefs.HasKey(LegacyHighScoreKey))
        {
            int legacyHighScore = PlayerPrefs.GetInt(LegacyHighScoreKey, 0);
            if (legacyHighScore > CurrentSave.highScore)
            {
                CurrentSave.highScore = legacyHighScore;
                changed = true;
            }
        }

        if (CurrentSave.totalScore == 0 && PlayerPrefs.HasKey(LegacyTotalScoreKey))
        {
            CurrentSave.totalScore = PlayerPrefs.GetInt(LegacyTotalScoreKey, 0);
            changed = true;
        }

        if (string.IsNullOrEmpty(CurrentSave.selectedSkinId) && PlayerPrefs.HasKey(LegacySelectedSkinKey))
        {
            CurrentSave.selectedSkinId = PlayerPrefs.GetString(LegacySelectedSkinKey, "");
            changed = true;
        }

        if (CurrentSave.saveVersion < 2)
        {
            CurrentSave.saveVersion = 2;
            changed = true;
        }

        if (changed) SaveGame();
    }
}
