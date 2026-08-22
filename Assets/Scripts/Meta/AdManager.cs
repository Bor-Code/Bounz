using UnityEngine;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [SerializeField] private int gamesBeforeInterstitial = 3;
    private int _gamesPlayedSinceLastAd = 0;

    private Action _onRewardedAdCompleted;
    private Action _onRewardedAdFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        if (IAPManager.Instance != null && IAPManager.Instance.HasNoAds)
        {
            return;
        }

        _gamesPlayedSinceLastAd++;
        if (_gamesPlayedSinceLastAd >= gamesBeforeInterstitial)
        {
            ShowInterstitialAd();
        }
    }

    public void ShowInterstitialAd()
    {
        if (IAPManager.Instance != null && IAPManager.Instance.HasNoAds) return;

        // Gerçek projede Unity Ads, AppLovin, veya AdMob SDK interstitial çağırma kodu
        _gamesPlayedSinceLastAd = 0;
    }

    public void ShowRewardedAd(Action onSuccess, Action onFailed = null)
    {
        // Gerçek projede reklamın hazır olup olmadığı kontrol edilir
        _onRewardedAdCompleted = onSuccess;
        _onRewardedAdFailed = onFailed;

        // Simülasyon: Reklam başarıyla izlendi varsayıyoruz
        SimulateRewardedAdSuccess();
    }

    private void SimulateRewardedAdSuccess()
    {
        _onRewardedAdCompleted?.Invoke();
        _onRewardedAdCompleted = null;
        _onRewardedAdFailed = null;
    }

    public void ShowBannerAd()
    {
        if (IAPManager.Instance != null && IAPManager.Instance.HasNoAds) return;
        // Alt/üst banner reklam gösterme
    }

    public void HideBannerAd()
    {
        // Banner reklam gizleme
    }
}
