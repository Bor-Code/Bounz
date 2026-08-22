using UnityEngine;
using System;
using System.Collections.Generic;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    public enum QuestType { CollectCoins, PerfectLandings, PlayGames }
    [Serializable]
    public class Quest
    {
        public string id;
        public QuestType type;
        public int targetAmount;
        public int reward;
        public int currentProgress;
        public bool isCompleted;
        public bool isRewardClaimed;
    }
    [Header("Active Quests")]
    public List<Quest> activeQuests = new List<Quest>();
    private const string QuestsSaveKey = "SavedQuests";
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadQuests();
    }
    private void OnEnable()
    {
        GameEvents.OnCoinCollected += HandleCoinCollected;
        GameEvents.OnPerfectLanding += HandlePerfectLanding;
        GameEvents.OnGameStarted += HandleGameStarted;
    }
    private void OnDisable()
    {
        GameEvents.OnCoinCollected -= HandleCoinCollected;
        GameEvents.OnPerfectLanding -= HandlePerfectLanding;
        GameEvents.OnGameStarted -= HandleGameStarted;
    }
    private void HandleCoinCollected(Vector3 _) => AddProgress(QuestType.CollectCoins, 1);
    private void HandlePerfectLanding(int combo, Vector3 _) => AddProgress(QuestType.PerfectLandings, 1);
    private void HandleGameStarted() => AddProgress(QuestType.PlayGames, 1);
    private void AddProgress(QuestType type, int amount)
    {
        bool updated = false;
        foreach (var quest in activeQuests)
        {
            if (quest.type == type && !quest.isCompleted)
            {
                quest.currentProgress += amount;
                if (quest.currentProgress >= quest.targetAmount)
                {
                    quest.currentProgress = quest.targetAmount;
                    quest.isCompleted = true;
                }
                updated = true;
            }
        }
        if (updated) SaveQuests();
    }
    public void ClaimReward(string questId)
    {
        foreach (var quest in activeQuests)
        {
            if (quest.id == questId && quest.isCompleted && !quest.isRewardClaimed)
            {
                quest.isRewardClaimed = true;
                if (SkinManager.Instance != null)
                {
                    SkinManager.Instance.AddTotalScore(quest.reward);
                }
                SaveQuests();
                break;
            }
        }
    }
    private void SaveQuests()
    {
        string json = JsonUtility.ToJson(new QuestSaveData { quests = activeQuests });
        PlayerPrefs.SetString(QuestsSaveKey, json);
        PlayerPrefs.Save();
    }
    private void LoadQuests()
    {
        string json = PlayerPrefs.GetString(QuestsSaveKey, "");
        if (string.IsNullOrEmpty(json))
        {
            GenerateDefaultQuests();
        }
        else
        {
            QuestSaveData data = JsonUtility.FromJson<QuestSaveData>(json);
            activeQuests = data.quests;
        }
    }
    private void GenerateDefaultQuests()
    {
        activeQuests = new List<Quest>
        {
            new Quest { id = "q1", type = QuestType.CollectCoins, targetAmount = 50, reward = 100 },
            new Quest { id = "q2", type = QuestType.PerfectLandings, targetAmount = 10, reward = 150 },
            new Quest { id = "q3", type = QuestType.PlayGames, targetAmount = 5, reward = 50 }
        };
        SaveQuests();
    }
    [Serializable]
    private class QuestSaveData
    {
        public List<Quest> quests;
    }
}