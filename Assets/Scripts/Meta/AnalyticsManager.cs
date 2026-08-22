using UnityEngine;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private int _sessionGameCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

    private void LogEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        // Burada gerçek projede GameAnalytics/Firebase SDK metodları çağrılır.
        string paramStr = "";
        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                paramStr += $"[{kvp.Key}: {kvp.Value}] ";
            }
        }
        Debug.Log($"[Analytics] Event: {eventName} | Parameters: {paramStr}");
    }
}
