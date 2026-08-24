using UnityEngine;
using System;

public class OfflineEarningsManager : MonoBehaviour
{
    public static OfflineEarningsManager Instance { get; private set; }

    [SerializeField] private int coinsPerMinute = 1;
    [SerializeField] private int maxOfflineMinutes = 1440; // Maksimum 24 saat birikebilir
    [SerializeField] private int minMinutesToClaim = 5;

    private const string LastSessionTimeKey = "LastSessionTime";

    public int PendingOfflineCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        CalculateOfflineEarnings();
    }

    private void CalculateOfflineEarnings()
    {
        string lastSessionStr = SaveManager.GetStringValue(LastSessionTimeKey, "");
        if (string.IsNullOrEmpty(lastSessionStr))
        {
            UpdateLastSessionTime();
            return;
        }

        if (long.TryParse(lastSessionStr, out long lastSessionBinary))
        {
            DateTime lastSessionDate = DateTime.FromBinary(lastSessionBinary);
            TimeSpan timePassed = DateTime.Now - lastSessionDate;
            
            int minutesPassed = (int)timePassed.TotalMinutes;

            if (minutesPassed >= minMinutesToClaim)
            {
                int effectiveMinutes = Mathf.Min(minutesPassed, maxOfflineMinutes);
                PendingOfflineCoins = effectiveMinutes * coinsPerMinute;
            }
        }
    }

    public void ClaimOfflineEarnings()
    {
        if (PendingOfflineCoins > 0 && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += PendingOfflineCoins;
            SaveManager.Instance.SaveGame();
            PendingOfflineCoins = 0;
            UpdateLastSessionTime();
        }
    }

    private void OnApplicationQuit()
    {
        UpdateLastSessionTime();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            UpdateLastSessionTime();
        }
    }

    private void UpdateLastSessionTime()
    {
        SaveManager.SetStringValue(LastSessionTimeKey, DateTime.Now.ToBinary().ToString());
    }
}
