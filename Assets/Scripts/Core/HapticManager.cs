using UnityEngine;
public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }
    [Header("Titreşim Ayarları")]
    [Tooltip("Titreşimleri tamamen kapat (erişilebilirlik seçeneği).")]
    [SerializeField] private bool hapticsEnabled = true;
    private const long VibShort  = 20L;   
    private const long VibMedium = 50L;   
    private const long VibLong   = 120L;  
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerJumped   += HandleJump;
        GameEvents.OnPlayerLanded   += HandleLand;
        GameEvents.OnSpringBounced  += HandleSpring;
        GameEvents.OnPlatformBroken += HandleBreak;
        GameEvents.OnPlayerDied     += HandleDeath;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerJumped   -= HandleJump;
        GameEvents.OnPlayerLanded   -= HandleLand;
        GameEvents.OnSpringBounced  -= HandleSpring;
        GameEvents.OnPlatformBroken -= HandleBreak;
        GameEvents.OnPlayerDied     -= HandleDeath;
    }
    private void HandleJump(float ratio)
    {
        Vibrate(ratio > 0.6f ? VibShort * 2 : VibShort);
    }
    private void HandleLand(float impactSpeed)
    {
        if (impactSpeed > 6f)
            Vibrate(VibMedium);
        else if (impactSpeed > 3f)
            Vibrate(VibShort);
    }
    private void HandleSpring()             => Vibrate(VibShort * 2);
    private void HandleBreak(Vector3 _)     => Vibrate(VibMedium);
    private void HandleDeath(Vector3 _)     => Vibrate(VibLong);
    public bool HapticsEnabled
    {
        get => hapticsEnabled;
        set => hapticsEnabled = value;
    }
    private void Vibrate(long milliseconds)
    {
        if (!hapticsEnabled) return;
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using var vibrator = new AndroidJavaClass("com.unity3d.player.UnityPlayer")
                .GetStatic<AndroidJavaObject>("currentActivity")
                .Call<AndroidJavaObject>("getSystemService", "vibrator");
            vibrator.Call("vibrate", milliseconds);
        }
        catch {  }
#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
    }
}