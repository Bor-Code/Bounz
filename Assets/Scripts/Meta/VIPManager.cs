using UnityEngine;
using System;

public class VIPManager : MonoBehaviour
{
    public static VIPManager Instance { get; private set; }

    private const string VIPSaveKey = "IsVIPActive";
    private const string VIPExpiryKey = "VIPExpiryDate";
    private const string LastDailyRewardKey = "VIPLastDailyRewardDate";

    public bool IsVIPActive { get; private set; }
    
    [SerializeField] private int dailyVipCoinReward = 500;
    [SerializeField] private string vipExclusiveSkinId = "vip_king_skin";

    public event Action OnVIPStatusChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        CheckVIPStatus();
    }

    private void CheckVIPStatus()
    {
        IsVIPActive = PlayerPrefs.GetInt(VIPSaveKey, 0) == 1;

        if (IsVIPActive)
        {
            string expiryStr = PlayerPrefs.GetString(VIPExpiryKey, "");
            if (long.TryParse(expiryStr, out long expiryBinary))
            {
                DateTime expiryDate = DateTime.FromBinary(expiryBinary);
                if (DateTime.Now >= expiryDate)
                {
                    // VIP süresi dolmuş
                    IsVIPActive = false;
                    PlayerPrefs.SetInt(VIPSaveKey, 0);
                    PlayerPrefs.Save();
                }
                else
                {
                    // VIP devam ediyor, günlük ödül kontrolü
                    CheckDailyReward();
                }
            }
        }
        
        OnVIPStatusChanged?.Invoke();
    }

    private void CheckDailyReward()
    {
        string lastRewardStr = PlayerPrefs.GetString(LastDailyRewardKey, "");
        bool canClaim = false;

        if (string.IsNullOrEmpty(lastRewardStr))
        {
            canClaim = true;
        }
        else if (long.TryParse(lastRewardStr, out long lastRewardBinary))
        {
            DateTime lastRewardDate = DateTime.FromBinary(lastRewardBinary);
            if (DateTime.Now.Date > lastRewardDate.Date)
            {
                canClaim = true;
            }
        }

        if (canClaim && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += dailyVipCoinReward;
            SaveManager.Instance.SaveGame();
            
            PlayerPrefs.SetString(LastDailyRewardKey, DateTime.Now.ToBinary().ToString());
            PlayerPrefs.Save();
            
            Debug.Log($"[VIP] Günlük {dailyVipCoinReward} Coin VIP ödülü verildi!");
        }
    }

    // Gerçekte IAP (In-App Purchasing) ile tetiklenir (Örn: 1 Aylık Abonelik Satın Alındı)
    public void PurchaseVIP(int daysDuration = 30)
    {
        IsVIPActive = true;
        DateTime expiry = DateTime.Now.AddDays(daysDuration);
        
        PlayerPrefs.SetInt(VIPSaveKey, 1);
        PlayerPrefs.SetString(VIPExpiryKey, expiry.ToBinary().ToString());
        PlayerPrefs.Save();

        // VIP Özel Skin ver
        if (SaveManager.Instance != null && !SaveManager.Instance.CurrentSave.unlockedSkins.Contains(vipExclusiveSkinId))
        {
            SaveManager.Instance.CurrentSave.unlockedSkins.Add(vipExclusiveSkinId);
            SaveManager.Instance.SaveGame();
        }

        OnVIPStatusChanged?.Invoke();
        CheckDailyReward();
    }
}
