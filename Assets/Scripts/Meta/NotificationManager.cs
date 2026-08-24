

using UnityEngine;



public class NotificationManager : MonoBehaviour

{

    public static NotificationManager Instance { get; private set; }



    [SerializeField] private string title = "Bounz Seni Bekliyor!";

    [SerializeField] private string message = "Günlük ödülünü almak için oyuna dön!";

    [SerializeField] private int hoursToWait = 24;

    private INotificationService _notificationService;



    private void Awake()

    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        _notificationService = new LocalNotificationService();

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



    public void SetService(INotificationService notificationService)

    {

        _notificationService = notificationService ?? new LocalNotificationService();

    }



    private void ScheduleNotification()

    {

        _notificationService?.ScheduleReturnNotification(title, message, hoursToWait);

    }



    private void CancelNotifications()

    {

        _notificationService?.CancelAll();

    }

}

