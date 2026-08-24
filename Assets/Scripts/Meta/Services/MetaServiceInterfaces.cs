

using System;

using System.Collections.Generic;

using UnityEngine;



public interface IAdService

{

    bool IsRewardedAdReady { get; }

    void ShowInterstitial();

    void ShowRewarded(Action onSuccess, Action onFailed);

    void ShowBanner();

    void HideBanner();

}



public class LocalAdService : IAdService

{

    public bool IsRewardedAdReady => true;



    public void ShowInterstitial() { }



    public void ShowRewarded(Action onSuccess, Action onFailed)

    {

        if (IsRewardedAdReady) onSuccess?.Invoke();

        else onFailed?.Invoke();

    }



    public void ShowBanner() { }

    public void HideBanner() { }

}



public interface IIAPService

{

    void Purchase(string productId, Action<string> onSuccess, Action<string> onFailed);

}



public class LocalIAPService : IIAPService

{

    public void Purchase(string productId, Action<string> onSuccess, Action<string> onFailed)

    {

        if (string.IsNullOrEmpty(productId)) onFailed?.Invoke(productId);

        else onSuccess?.Invoke(productId);

    }

}



public interface IAnalyticsService

{

    void LogEvent(string eventName, Dictionary<string, object> parameters = null);

}



public class DebugAnalyticsService : IAnalyticsService

{

    public void LogEvent(string eventName, Dictionary<string, object> parameters = null)

    {

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



public interface INotificationService

{

    void ScheduleReturnNotification(string title, string message, int hoursToWait);

    void CancelAll();

}



public class LocalNotificationService : INotificationService

{

    public void ScheduleReturnNotification(string title, string message, int hoursToWait)

    {

        Debug.Log($"[Notification] Scheduled '{title}: {message}' for {hoursToWait} hours later.");

    }



    public void CancelAll()

    {

        Debug.Log("[Notification] Canceled pending notifications.");

    }

}

