using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class BattlePassTier
{
    public int requiredTokens;
    public int freeCoinReward;
    public int premiumCoinReward;
    public bool isFreeClaimed;
    public bool isPremiumClaimed;
}

public class BattlePassManager : MonoBehaviour
{
    public static BattlePassManager Instance { get; private set; }

    [SerializeField] private List<BattlePassTier> tiers = new List<BattlePassTier>();
    
    private const string TokensSaveKey = "BattlePassTokens";
    private const string PremiumSaveKey = "IsPremiumPassOwned";
    private const string ClaimsSaveKey = "BattlePassClaims_";

    public int CurrentTokens { get; private set; }
    public bool IsPremiumOwned { get; private set; }

    public event Action OnPassUpdated;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadPassData();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void LoadPassData()
    {
        CurrentTokens = PlayerPrefs.GetInt(TokensSaveKey, 0);
        IsPremiumOwned = PlayerPrefs.GetInt(PremiumSaveKey, 0) == 1;

        if (tiers.Count == 0)
        {
            tiers.Add(new BattlePassTier { requiredTokens = 100, freeCoinReward = 50, premiumCoinReward = 500 });
            tiers.Add(new BattlePassTier { requiredTokens = 300, freeCoinReward = 100, premiumCoinReward = 1000 });
            tiers.Add(new BattlePassTier { requiredTokens = 600, freeCoinReward = 250, premiumCoinReward = 2000 });
        }

        for (int i = 0; i < tiers.Count; i++)
        {
            tiers[i].isFreeClaimed = PlayerPrefs.GetInt(ClaimsSaveKey + "Free_" + i, 0) == 1;
            tiers[i].isPremiumClaimed = PlayerPrefs.GetInt(ClaimsSaveKey + "Prem_" + i, 0) == 1;
        }
    }

    private void SavePassData()
    {
        PlayerPrefs.SetInt(TokensSaveKey, CurrentTokens);
        PlayerPrefs.SetInt(PremiumSaveKey, IsPremiumOwned ? 1 : 0);
        
        for (int i = 0; i < tiers.Count; i++)
        {
            PlayerPrefs.SetInt(ClaimsSaveKey + "Free_" + i, tiers[i].isFreeClaimed ? 1 : 0);
            PlayerPrefs.SetInt(ClaimsSaveKey + "Prem_" + i, tiers[i].isPremiumClaimed ? 1 : 0);
        }
        PlayerPrefs.Save();
        
        OnPassUpdated?.Invoke();
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        int tokensEarned = finalScore / 20; 
        if (tokensEarned > 0)
        {
            CurrentTokens += tokensEarned;
            SavePassData();
        }
    }

    public void BuyPremiumPass()
    {
        if (!IsPremiumOwned)
        {
            IsPremiumOwned = true;
            SavePassData();
        }
    }

    public void ClaimFreeReward(int tierIndex)
    {
        if (tierIndex >= 0 && tierIndex < tiers.Count)
        {
            BattlePassTier tier = tiers[tierIndex];
            if (CurrentTokens >= tier.requiredTokens && !tier.isFreeClaimed)
            {
                tier.isFreeClaimed = true;
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.CurrentSave.totalScore += tier.freeCoinReward;
                    SaveManager.Instance.SaveGame();
                }
                SavePassData();
            }
        }
    }

    public void ClaimPremiumReward(int tierIndex)
    {
        if (tierIndex >= 0 && tierIndex < tiers.Count)
        {
            BattlePassTier tier = tiers[tierIndex];
            if (IsPremiumOwned && CurrentTokens >= tier.requiredTokens && !tier.isPremiumClaimed)
            {
                tier.isPremiumClaimed = true;
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.CurrentSave.totalScore += tier.premiumCoinReward;
                    SaveManager.Instance.SaveGame();
                }
                SavePassData();
            }
        }
    }
}
