using UnityEngine;
using System;
public class DailyRewardManager : MonoBehaviour
{
    public static DailyRewardManager Instance { get; private set; }
    [SerializeField] private int[] dailyRewards = new int[] { 50, 100, 150, 200, 250, 300, 500 };
    private const string LastClaimTimeKey = "LastDailyRewardClaimTime";
    private const string CurrentStreakKey = "DailyRewardStreak";
    public bool CanClaimReward { get; private set; }
    public int CurrentStreak { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CheckRewardStatus();
    }
    private void CheckRewardStatus()
    {
        string lastClaimStr = SaveManager.GetStringValue(LastClaimTimeKey, "");
        CurrentStreak = SaveManager.GetIntValue(CurrentStreakKey, 0);
        if (string.IsNullOrEmpty(lastClaimStr))
        {
            CanClaimReward = true;
            return;
        }
        if (long.TryParse(lastClaimStr, out long lastClaimBinary))
        {
            DateTime lastClaimDate = DateTime.FromBinary(lastClaimBinary).Date;
            DateTime currentDate = DateTime.Now.Date;
            int daysPassed = (currentDate - lastClaimDate).Days;
            if (daysPassed == 1)
            {
                CanClaimReward = true;
            }
            else if (daysPassed > 1)
            {
                CanClaimReward = true;
                CurrentStreak = 0;
            }
            else
            {
                CanClaimReward = false;
            }
        }
    }
    public int GetNextRewardAmount()
    {
        int index = Mathf.Min(CurrentStreak, dailyRewards.Length - 1);
        return dailyRewards[index];
    }
    public bool ClaimReward()
    {
        if (!CanClaimReward) return false;
        int rewardAmount = GetNextRewardAmount();
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += rewardAmount;
            SaveManager.Instance.SaveGame();
        }
        CurrentStreak++;
        if (CurrentStreak >= dailyRewards.Length)
        {
            CurrentStreak = 0;
        }
        SaveManager.SetIntValue(CurrentStreakKey, CurrentStreak);
        SaveManager.SetStringValue(LastClaimTimeKey, DateTime.Now.ToBinary().ToString());
        CanClaimReward = false;
        return true;
    }
}
