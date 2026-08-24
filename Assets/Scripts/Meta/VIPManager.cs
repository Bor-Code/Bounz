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
        IsVIPActive = SaveManager.GetIntValue(VIPSaveKey, 0) == 1;

        if (IsVIPActive)
        {
            string expiryStr = SaveManager.GetStringValue(VIPExpiryKey, "");
            if (long.TryParse(expiryStr, out long expiryBinary))
            {
                DateTime expiryDate = DateTime.FromBinary(expiryBinary);
                if (DateTime.Now >= expiryDate)
                {
                    IsVIPActive = false;
                    SaveManager.SetIntValue(VIPSaveKey, 0);
                }
                else
                {
                    CheckDailyReward();
                }
            }
        }

        OnVIPStatusChanged?.Invoke();
    }

    private void CheckDailyReward()
    {
        string lastRewardStr = SaveManager.GetStringValue(LastDailyRewardKey, "");
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
            SaveManager.SetStringValue(LastDailyRewardKey, DateTime.Now.ToBinary().ToString());
            Debug.Log($"[VIP] Günlük {dailyVipCoinReward} Coin VIP ödülü verildi!");
        }
    }

    public void PurchaseVIPWithIAP(int daysDuration = 30)
    {
        if (IAPManager.Instance != null)
        {
            IAPManager.Instance.BuyProduct(IAPManager.PRODUCT_VIP_MONTHLY, () => PurchaseVIP(daysDuration));
        }
        else
        {
            PurchaseVIP(daysDuration);
        }
    }

    public void PurchaseVIP(int daysDuration = 30)
    {
        IsVIPActive = true;
        DateTime expiry = DateTime.Now.AddDays(daysDuration);

        SaveManager.SetIntValue(VIPSaveKey, 1);
        SaveManager.SetStringValue(VIPExpiryKey, expiry.ToBinary().ToString());

        if (SaveManager.Instance != null && !SaveManager.Instance.CurrentSave.unlockedSkins.Contains(vipExclusiveSkinId))
        {
            SaveManager.Instance.CurrentSave.unlockedSkins.Add(vipExclusiveSkinId);
            SaveManager.Instance.SaveGame();
        }

        OnVIPStatusChanged?.Invoke();
        CheckDailyReward();
    }
}
