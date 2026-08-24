using UnityEngine;
using System;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }

    public const string PRODUCT_COIN_PACK_SMALL = "com.bounz.coin_pack_small";
    public const string PRODUCT_COIN_PACK_LARGE = "com.bounz.coin_pack_large";
    public const string PRODUCT_NO_ADS = "com.bounz.no_ads";
    public const string PRODUCT_PIGGY_BANK_SMASH = "com.bounz.piggybank_smash";
    public const string PRODUCT_VIP_MONTHLY = "com.bounz.vip_monthly";

    private const string NoAdsSaveKey = "IsNoAdsPurchased";

    public bool HasNoAds { get; private set; }
    private IIAPService _iapService;

    public event Action<string> OnPurchaseSucceeded;
    public event Action<string> OnPurchaseFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _iapService = new LocalIAPService();
        LoadIAPStatus();
    }

    private void LoadIAPStatus()
    {
        HasNoAds = SaveManager.GetIntValue(NoAdsSaveKey, 0) == 1;
    }

    public void SetService(IIAPService iapService)
    {
        _iapService = iapService ?? new LocalIAPService();
    }

    public void BuyProduct(string productId, Action onSuccess = null, Action onFailed = null)
    {
        _iapService ??= new LocalIAPService();
        _iapService.Purchase(
            productId,
            purchasedId =>
            {
                ProcessPurchaseSuccess(purchasedId);
                onSuccess?.Invoke();
            },
            failedId =>
            {
                ProcessPurchaseFailed(failedId);
                onFailed?.Invoke();
            });
    }

    private void ProcessPurchaseSuccess(string productId)
    {
        switch (productId)
        {
            case PRODUCT_COIN_PACK_SMALL:
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.CurrentSave.totalScore += 1000;
                    SaveManager.Instance.SaveGame();
                }
                break;

            case PRODUCT_COIN_PACK_LARGE:
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.CurrentSave.totalScore += 5000;
                    SaveManager.Instance.SaveGame();
                }
                break;

            case PRODUCT_NO_ADS:
                HasNoAds = true;
                SaveManager.SetIntValue(NoAdsSaveKey, 1);
                break;
        }

        AnalyticsManager.Instance?.LogIAPPurchased(productId);
        OnPurchaseSucceeded?.Invoke(productId);
    }

    private void ProcessPurchaseFailed(string productId)
    {
        AnalyticsManager.Instance?.LogEvent("iap_failed", new System.Collections.Generic.Dictionary<string, object>
        {
            { "product_id", productId }
        });
        OnPurchaseFailed?.Invoke(productId);
    }
}
