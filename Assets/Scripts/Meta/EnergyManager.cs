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
        CurrentEnergy = SaveManager.GetIntValue(EnergySaveKey, maxEnergy);
        CheckEnergyRegen();
    }

    private void CheckEnergyRegen()
    {
        string lastRegenStr = SaveManager.GetStringValue(LastEnergyTimeKey, "");
        if (string.IsNullOrEmpty(lastRegenStr))
        {
            UpdateLastRegenTime(DateTime.Now);
            return;
        }

        if (long.TryParse(lastRegenStr, out long lastRegenBinary))
        {
            DateTime lastRegenDate = DateTime.FromBinary(lastRegenBinary);
            TimeSpan timePassed = DateTime.Now - lastRegenDate;

            if (CurrentEnergy < maxEnergy)
            {
                int energyToGive = (int)(timePassed.TotalMinutes / minutesToRegenOneEnergy);
                
                if (energyToGive > 0)
                {
                    CurrentEnergy = Mathf.Min(CurrentEnergy + energyToGive, maxEnergy);
                    
                    DateTime newRegenDate = lastRegenDate.AddMinutes(energyToGive * minutesToRegenOneEnergy);
                    UpdateLastRegenTime(newRegenDate);
                    SaveEnergy();
                }
                else
                {
                    double remainingMinutes = minutesToRegenOneEnergy - timePassed.TotalMinutes;
                    TimeUntilNextEnergy = TimeSpan.FromMinutes(remainingMinutes);
                    OnEnergyUpdated?.Invoke();
                }
            }
        }
    }

    private void UpdateLastRegenTime(DateTime time)
    {
        SaveManager.SetStringValue(LastEnergyTimeKey, time.ToBinary().ToString());
    }

    private void SaveEnergy()
    {
        SaveManager.SetIntValue(EnergySaveKey, CurrentEnergy);
        OnEnergyUpdated?.Invoke();
    }

    public bool ConsumeEnergy()
    {
        if (CurrentEnergy > 0)
        {
            if (CurrentEnergy == maxEnergy)
            {
                UpdateLastRegenTime(DateTime.Now);
            }
            
            CurrentEnergy--;
            SaveEnergy();
            return true;
        }
        return false;
    }

    public void RefillEnergyWithCoins()
    {
        if (CurrentEnergy >= maxEnergy) return;

        if (SaveManager.Instance != null && SaveManager.Instance.CurrentSave.totalScore >= refillCost)
        {
            SaveManager.Instance.CurrentSave.totalScore -= refillCost;
            SaveManager.Instance.SaveGame();
            
            CurrentEnergy = maxEnergy;
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
                    SaveEnergy();
                }
            );
        }
    }
}
