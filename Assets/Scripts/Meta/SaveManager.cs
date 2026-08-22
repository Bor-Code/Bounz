using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int highScore;
    public int totalScore;
    public string selectedSkinId;
    public List<string> unlockedSkins = new List<string>();
    public string questDataJson;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "BounzGameSave";
    
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
            CurrentSave = JsonUtility.FromJson<GameSaveData>(json);
        }
    }

    public void SaveGame()
    {
        string json = JsonUtility.ToJson(CurrentSave);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }
}
