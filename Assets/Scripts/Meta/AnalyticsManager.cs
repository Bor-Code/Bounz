

using UnityEngine;

using System.Collections.Generic;



public class AnalyticsManager : MonoBehaviour

{

    public static AnalyticsManager Instance { get; private set; }



    private int _sessionGameCount = 0;

    private IAnalyticsService _analyticsService;



    private void Awake()

    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _analyticsService = new DebugAnalyticsService();

    }



    private void OnEnable()

    {

        GameEvents.OnGameStarted += HandleGameStarted;

        ScoreEvents.OnGameOver += HandleGameOver;

    }



    private void OnDisable()

    {

        GameEvents.OnGameStarted -= HandleGameStarted;

        ScoreEvents.OnGameOver -= HandleGameOver;

    }



    private void HandleGameStarted()

    {

        _sessionGameCount++;

        LogEvent("game_started", new Dictionary<string, object>

        {

            { "session_game_index", _sessionGameCount }

        });

    }



    private void HandleGameOver(int finalScore, bool isNewHighScore)

    {

        LogEvent("game_over", new Dictionary<string, object>

        {

            { "final_score", finalScore },

            { "is_new_high_score", isNewHighScore }

        });

    }



    public void SetService(IAnalyticsService analyticsService)

    {

        _analyticsService = analyticsService ?? new DebugAnalyticsService();

    }



    public void LogSkinPurchased(string skinId, int price)

    {

        LogEvent("skin_purchased", new Dictionary<string, object>

        {

            { "skin_id", skinId },

            { "price", price }

        });

    }



    public void LogIAPPurchased(string productId)

    {

        LogEvent("iap_purchased", new Dictionary<string, object>

        {

            { "product_id", productId }

        });

    }



    public void LogEvent(string eventName, Dictionary<string, object> parameters = null)

    {

        _analyticsService ??= new DebugAnalyticsService();

        _analyticsService.LogEvent(eventName, parameters);

    }

}

