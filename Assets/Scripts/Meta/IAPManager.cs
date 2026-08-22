using UnityEngine;
using System;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }

    public const string PRODUCT_COIN_PACK_SMALL = "com.bounz.coin_pack_small";
    public const string PRODUCT_COIN_PACK_LARGE = "com.bounz.coin_pack_large";
    public const string PRODUCT_NO_ADS = "com.bounz.no_ads";

    private const string NoAdsSaveKey = "IsNoAdsPurchased";

    public bool HasNoAds { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadIAPStatus();
    }

    private void LoadIAPStatus()
    {
        HasNoAds = PlayerPrefs.GetInt(NoAdsSaveKey, 0) == 1;
    }

    // Gerçek bir projede Unity IAP kullanılarak bu metod satın alma başarılı olduğunda tetiklenir.
    public void BuyProduct(string productId)
    {
        // Satın alma işlemini başlatma simülasyonu
        ProcessPurchaseSuccess(productId);
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
                PlayerPrefs.SetInt(NoAdsSaveKey, 1);
                PlayerPrefs.Save();
                break;
        }

        if (AnalyticsManager.Instance != null)
        {
            AnalyticsManager.Instance.LogIAPPurchased(productId);
        }
    }
}
