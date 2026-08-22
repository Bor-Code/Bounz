using UnityEngine;

/// <summary>
/// Mobil cihazlarda dokunsal geri bildirim (vibrasyon) sağlar.
/// GameEvents event'lerini dinler; hiçbir gameplay script'ine bağımlı değildir.
///
/// Android ve iOS'ta Handheld.Vibrate() çalışır.
/// Editor'da çalışmaz (derleme hatası vermez, sadece sessiz kalır).
///
/// Sahneye boş bir GameObject ekle → HapticManager bileşenini ata.
/// </summary>
public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    [Header("Titreşim Ayarları")]
    [Tooltip("Titreşimleri tamamen kapat (erişilebilirlik seçeneği).")]
    [SerializeField] private bool hapticsEnabled = true;

    // Android için farklı titreşim süreleri (ms) — 
    // iOS Handheld.Vibrate() sabit kısa bir titreşim yapar.
    // Android özel süreler için AndroidJavaObject kullanılır.
    private const long VibShort  = 20L;   // jump / spring
    private const long VibMedium = 50L;   // land heavy
    private const long VibLong   = 120L;  // game over

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

    // ── Handlers ─────────────────────────────────────────────────────────────

    private void HandleJump(float ratio)
    {
        // Güçlü şarjda biraz daha uzun titreşim
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

    // ── Platform ─────────────────────────────────────────────────────────────

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
        catch { /* Titreşim desteklenmiyorsa sessizce geç */ }

#elif UNITY_IOS && !UNITY_EDITOR
        Handheld.Vibrate();

#endif
    }
}
