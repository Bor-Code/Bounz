using UnityEngine;
using System;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private const string XPSaveKey = "PlayerXP";
    private const string LevelSaveKey = "PlayerLevel";

    public int CurrentXP { get; private set; }
    public int CurrentLevel { get; private set; }

    public event Action<int> OnLevelUp;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadProfile();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void LoadProfile()
    {
        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave != null)
        {
            CurrentLevel = PlayerPrefs.GetInt(LevelSaveKey, 1);
            CurrentXP = PlayerPrefs.GetInt(XPSaveKey, 0);
        }
        else
        {
            CurrentLevel = PlayerPrefs.GetInt(LevelSaveKey, 1);
            CurrentXP = PlayerPrefs.GetInt(XPSaveKey, 0);
        }
    }

    private void SaveProfile()
    {
        PlayerPrefs.SetInt(LevelSaveKey, CurrentLevel);
        PlayerPrefs.SetInt(XPSaveKey, CurrentXP);
        PlayerPrefs.Save();
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        int xpEarned = finalScore / 10;
        if (xpEarned > 0)
        {
            AddXP(xpEarned);
        }
    }

    public void AddXP(int amount)
    {
        CurrentXP += amount;
        CheckLevelUp();
        SaveProfile();
    }

    private void CheckLevelUp()
    {
        int requiredXP = GetXPForNextLevel();
        bool leveledUp = false;

        while (CurrentXP >= requiredXP)
        {
            CurrentXP -= requiredXP;
            CurrentLevel++;
            leveledUp = true;
            requiredXP = GetXPForNextLevel();
            
            GiveLevelUpReward();
        }

        if (leveledUp)
        {
            OnLevelUp?.Invoke(CurrentLevel);
        }
    }

    public int GetXPForNextLevel()
    {
        return CurrentLevel * 1000;
    }

    private void GiveLevelUpReward()
    {
        int rewardCoins = CurrentLevel * 100;
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += rewardCoins;
            SaveManager.Instance.SaveGame();
        }
    }
}
