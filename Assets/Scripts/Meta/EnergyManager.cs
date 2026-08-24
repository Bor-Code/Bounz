using UnityEngine;
using System;

public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }

    private const string EnergySaveKey = "PlayerEnergy";
    private const string LastEnergyTimeKey = "LastEnergyRegenTime";

    [SerializeField] private int maxEnergy = 5;
    [SerializeField] private int minutesToRegenOneEnergy = 10;
    [SerializeField] private int refillCost = 500;

    public int CurrentEnergy { get; private set; }
    public int MaxEnergy => maxEnergy;
    public int RefillCost => refillCost;
    public bool HasEnergy => CurrentEnergy > 0;
    public TimeSpan TimeUntilNextEnergy { get; private set; }

    public event Action OnEnergyUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadEnergy();
    }

    private void Update()
    {
        if (CurrentEnergy < maxEnergy)
        {
            CheckEnergyRegen();
        }
    }

    private void LoadEnergy()
    {
        CurrentEnergy = Mathf.Clamp(SaveManager.GetIntValue(EnergySaveKey, maxEnergy), 0, maxEnergy);
        CheckEnergyRegen();
        NotifyEnergyUpdated();
    }

    private void CheckEnergyRegen()
    {
        if (CurrentEnergy >= maxEnergy)
        {
            TimeUntilNextEnergy = TimeSpan.Zero;
            return;
        }

        string lastRegenStr = SaveManager.GetStringValue(LastEnergyTimeKey, "");
        if (string.IsNullOrEmpty(lastRegenStr))
        {
            UpdateLastRegenTime(DateTime.Now);
            TimeUntilNextEnergy = TimeSpan.FromMinutes(minutesToRegenOneEnergy);
            NotifyEnergyUpdated();
            return;
        }

        if (!long.TryParse(lastRegenStr, out long lastRegenBinary))
        {
            UpdateLastRegenTime(DateTime.Now);
            TimeUntilNextEnergy = TimeSpan.FromMinutes(minutesToRegenOneEnergy);
            NotifyEnergyUpdated();
            return;
        }

        DateTime lastRegenDate = DateTime.FromBinary(lastRegenBinary);
        TimeSpan timePassed = DateTime.Now - lastRegenDate;
        if (timePassed < TimeSpan.Zero)
        {
            UpdateLastRegenTime(DateTime.Now);
            TimeUntilNextEnergy = TimeSpan.FromMinutes(minutesToRegenOneEnergy);
            NotifyEnergyUpdated();
            return;
        }

        int energyToGive = (int)(timePassed.TotalMinutes / minutesToRegenOneEnergy);
        if (energyToGive > 0)
        {
            CurrentEnergy = Mathf.Min(CurrentEnergy + energyToGive, maxEnergy);
            DateTime newRegenDate = CurrentEnergy >= maxEnergy
                ? DateTime.Now
                : lastRegenDate.AddMinutes(energyToGive * minutesToRegenOneEnergy);
            UpdateLastRegenTime(newRegenDate);
            SaveEnergy();
            return;
        }

        double remainingMinutes = Math.Max(0d, minutesToRegenOneEnergy - timePassed.TotalMinutes);
        TimeUntilNextEnergy = TimeSpan.FromMinutes(remainingMinutes);
        NotifyEnergyUpdated();
    }

    private void UpdateLastRegenTime(DateTime time)
    {
        SaveManager.SetStringValue(LastEnergyTimeKey, time.ToBinary().ToString());
    }

    private void SaveEnergy()
    {
        SaveManager.SetIntValue(EnergySaveKey, CurrentEnergy);
        NotifyEnergyUpdated();
    }

    private void NotifyEnergyUpdated()
    {
        OnEnergyUpdated?.Invoke();
    }

    public bool ConsumeEnergy()
    {
        if (CurrentEnergy <= 0)
        {
            NotifyEnergyUpdated();
            return false;
        }

        if (CurrentEnergy == maxEnergy)
        {
            UpdateLastRegenTime(DateTime.Now);
        }

        CurrentEnergy--;
        SaveEnergy();
        return true;
    }

    public void RefillEnergyWithCoins()
    {
        if (CurrentEnergy >= maxEnergy) return;

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave.totalScore >= refillCost)
        {
            SaveManager.Instance.CurrentSave.totalScore -= refillCost;
            SaveManager.Instance.SaveGame();

            CurrentEnergy = maxEnergy;
            TimeUntilNextEnergy = TimeSpan.Zero;
            SaveEnergy();
        }
    }

    public void RefillEnergyWithAd()
    {
        if (CurrentEnergy >= maxEnergy) return;

        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd(
                onSuccess: () =>
                {
                    CurrentEnergy = maxEnergy;
                    TimeUntilNextEnergy = TimeSpan.Zero;
                    SaveEnergy();
                }
            );
        }
    }
}
