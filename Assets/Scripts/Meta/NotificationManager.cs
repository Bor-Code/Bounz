using UnityEngine;
using System;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [SerializeField] private string title = "Bounz Seni Bekliyor!";
    [SerializeField] private string message = "Günlük ödülünü almak için oyuna dön!";
    [SerializeField] private int hoursToWait = 24;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            ScheduleNotification();
        }
        else
        {
            CancelNotifications();
        }
    }

    private void ScheduleNotification()
    {
        // Unity.Notifications package (Mobile Notifications) kullanılarak 
        // hoursToWait (24 saat) sonrasına bildirim planlanır.
        // Şimdilik sadece konsola logluyoruz.
        Debug.Log($"[NotificationManager] Scheduled '{title}: {message}' for {hoursToWait} hours later.");
    }

    private void CancelNotifications()
    {
        // Oyuncu oyuna girdiğinde bekleyen geri dönüş bildirimlerini iptal et.
        Debug.Log("[NotificationManager] Canceled pending notifications.");
    }
}
