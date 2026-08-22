using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Achievement
{
    public string id;
    public string title;
    public int targetScore;
    public bool isUnlocked;
    public int rewardAmount;
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    [SerializeField] private List<Achievement> achievements = new List<Achievement>();
    
    private const string AchSavePrefix = "Ach_";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAchievements();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void LoadAchievements()
    {
        if (achievements.Count == 0)
        {
            // Varsayılan başarımlar
            achievements.Add(new Achievement { id = "score_100", title = "Acemi Zıplayıcı (Skor 100)", targetScore = 100, rewardAmount = 50 });
            achievements.Add(new Achievement { id = "score_500", title = "Usta (Skor 500)", targetScore = 500, rewardAmount = 250 });
            achievements.Add(new Achievement { id = "score_1000", title = "Efsane (Skor 1000)", targetScore = 1000, rewardAmount = 1000 });
        }

        foreach (var ach in achievements)
        {
            ach.isUnlocked = PlayerPrefs.GetInt(AchSavePrefix + ach.id, 0) == 1;
        }
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        foreach (var ach in achievements)
        {
            if (!ach.isUnlocked && finalScore >= ach.targetScore)
            {
                UnlockAchievement(ach);
            }
        }
    }

    private void UnlockAchievement(Achievement ach)
    {
        ach.isUnlocked = true;
        PlayerPrefs.SetInt(AchSavePrefix + ach.id, 1);
        PlayerPrefs.Save();
        
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += ach.rewardAmount;
            SaveManager.Instance.SaveGame();
        }
    }
}
