

using UnityEngine;

using System;



public class AdManager : MonoBehaviour

{

    public static AdManager Instance { get; private set; }



    [SerializeField] private int gamesBeforeInterstitial = 3;

    private int _gamesPlayedSinceLastAd = 0;

    private IAdService _adService;



    private void Awake()

    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _adService = new LocalAdService();

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



    public void SetService(IAdService adService)

    {

        _adService = adService ?? new LocalAdService();

    }



    public void ShowInterstitialAd()

    {

        if (IAPManager.Instance != null && IAPManager.Instance.HasNoAds) return;

        _gamesPlayedSinceLastAd = 0;

        _adService?.ShowInterstitial();

    }



    public void ShowRewardedAd(Action onSuccess, Action onFailed = null)

    {

        _adService ??= new LocalAdService();

        _adService.ShowRewarded(onSuccess, onFailed);

    }



    public void ShowBannerAd()

    {

        if (IAPManager.Instance != null && IAPManager.Instance.HasNoAds) return;

        _adService?.ShowBanner();

    }



    public void HideBannerAd()

    {

        _adService?.HideBanner();

    }

}

