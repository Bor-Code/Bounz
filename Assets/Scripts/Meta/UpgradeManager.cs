using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class UpgradeStat
{
    public string id;
    public string displayName;
    public int currentLevel;
    public int maxLevel;
    public int baseCost;
    public float baseValue;
    public float valuePerLevel;

    public int GetNextCost()
    {
        return baseCost * (currentLevel + 1);
    }

    public float GetCurrentValue()
    {
        return baseValue + (currentLevel * valuePerLevel);
    }
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    public const string MagnetDurationUpgradeId = "magnet_duration";
    public const string ShieldDurationUpgradeId = "shield_duration";
    public const string CoinMultiplierUpgradeId = "coin_multiplier";

    [SerializeField] private List<UpgradeStat> upgrades = new List<UpgradeStat>();

    private const string UpgradeSavePrefix = "UpgradeLevel_";

    public event Action OnUpgradesChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeUpgrades();
    }

    private void InitializeUpgrades()
    {
        if (upgrades.Count == 0)
        {
            upgrades.Add(new UpgradeStat { id = MagnetDurationUpgradeId, displayName = "Mıknatıs Süresi", currentLevel = 0, maxLevel = 5, baseCost = 1000, baseValue = 5f, valuePerLevel = 1f });
            upgrades.Add(new UpgradeStat { id = ShieldDurationUpgradeId, displayName = "Kalkan Süresi", currentLevel = 0, maxLevel = 5, baseCost = 1000, baseValue = 5f, valuePerLevel = 1f });
            upgrades.Add(new UpgradeStat { id = CoinMultiplierUpgradeId, displayName = "Altın Çarpanı", currentLevel = 0, maxLevel = 5, baseCost = 1500, baseValue = 1f, valuePerLevel = 0.2f });
        }

        foreach (var upgrade in upgrades)
        {
            upgrade.currentLevel = SaveManager.GetIntValue(UpgradeSavePrefix + upgrade.id, 0);
        }
    }

    public List<UpgradeStat> GetAllUpgrades() => upgrades;

    public UpgradeStat GetUpgrade(string id)
    {
        return upgrades.Find(u => u.id == id);
    }

    public float GetUpgradeValue(string id, float fallback)
    {
        UpgradeStat stat = GetUpgrade(id);
        return stat != null ? stat.GetCurrentValue() : fallback;
    }

    public bool BuyUpgrade(string id)
    {
        UpgradeStat stat = GetUpgrade(id);
        if (stat == null || stat.currentLevel >= stat.maxLevel) return false;

        if (SaveManager.Instance != null)
        {
            int cost = stat.GetNextCost();
            if (SaveManager.Instance.CurrentSave.totalScore >= cost)
            {
                SaveManager.Instance.CurrentSave.totalScore -= cost;
                SaveManager.Instance.SaveGame();

                stat.currentLevel++;
                SaveManager.SetIntValue(UpgradeSavePrefix + stat.id, stat.currentLevel);

                OnUpgradesChanged?.Invoke();
                return true;
            }
        }
        return false;
    }
}
