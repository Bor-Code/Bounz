using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SpinReward
{
    public string rewardName;
    public int coinAmount;
    public float chanceWeight;
}

public class DailySpinManager : MonoBehaviour
{
    public static DailySpinManager Instance { get; private set; }

    private const string LastSpinTimeKey = "LastDailySpinTime";

    [SerializeField] private int hoursBetweenSpins = 24;
    [SerializeField] private List<SpinReward> rewards = new List<SpinReward>();

    public event Action<SpinReward> OnRewardSpun;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (rewards.Count == 0)
        {
            rewards.Add(new SpinReward { rewardName = "Küçük Kese", coinAmount = 100, chanceWeight = 50f });
            rewards.Add(new SpinReward { rewardName = "Orta Kese", coinAmount = 250, chanceWeight = 30f });
            rewards.Add(new SpinReward { rewardName = "Büyük Kese", coinAmount = 500, chanceWeight = 15f });
            rewards.Add(new SpinReward { rewardName = "Dev Hazine", coinAmount = 2500, chanceWeight = 5f });
        }
    }

    public bool CanSpin()
    {
        string lastSpinStr = PlayerPrefs.GetString(LastSpinTimeKey, "");
        if (string.IsNullOrEmpty(lastSpinStr))
        {
            return true;
        }

        if (long.TryParse(lastSpinStr, out long lastSpinBinary))
        {
            DateTime lastSpinDate = DateTime.FromBinary(lastSpinBinary);
            TimeSpan timePassed = DateTime.Now - lastSpinDate;
            return timePassed.TotalHours >= hoursBetweenSpins;
        }

        return true;
    }

    public void SpinWheel()
    {
        if (!CanSpin()) return;

        PlayerPrefs.SetString(LastSpinTimeKey, DateTime.Now.ToBinary().ToString());
        PlayerPrefs.Save();

        SpinReward wonReward = GetRandomReward();
        
        if (SaveManager.Instance != null && wonReward.coinAmount > 0)
        {
            SaveManager.Instance.CurrentSave.totalScore += wonReward.coinAmount;
            SaveManager.Instance.SaveGame();
        }

        OnRewardSpun?.Invoke(wonReward);
    }

    private SpinReward GetRandomReward()
    {
        float totalWeight = 0;
        foreach (var reward in rewards)
        {
            totalWeight += reward.chanceWeight;
        }

        float randomVal = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var reward in rewards)
        {
            currentWeight += reward.chanceWeight;
            if (randomVal <= currentWeight)
            {
                return reward;
            }
        }

        return rewards[0];
    }
}
