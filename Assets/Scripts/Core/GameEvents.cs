/// <summary>
/// Statik event bus — oyun içi tüm anlık olaylar buradan yayınlanır.
/// AudioManager ve ParticleSpawner bu event'leri dinler; hiçbiri
/// birbirine ya da PlayerController'a doğrudan bağımlı olmaz.
/// </summary>
public static class GameEvents
{
    // ── Hareket ───────────────────────────────────────────────────────────────
    /// <summary>Player zıpladığında. float = normalize kuvvet (0-1).</summary>
    public static event System.Action<float> OnPlayerJumped;

    /// <summary>Player platforma indiğinde. float = çarptığı hız (mutlak Y).</summary>
    public static event System.Action<float> OnPlayerLanded;

    // ── Platform ──────────────────────────────────────────────────────────────
    /// <summary>Spring platform player'ı fırlattığında.</summary>
    public static event System.Action OnSpringBounced;

    /// <summary>Fragile platform çöktüğünde. Vector3 = platform pozisyonu.</summary>
    public static event System.Action<UnityEngine.Vector3> OnPlatformBroken;

    /// <summary>SizeShift platform player'ın boyutunu değiştirdiğinde. float = yeni çarpan.</summary>
    public static event System.Action<float> OnSizeShifted;

    // ── Oyun Durumu ───────────────────────────────────────────────────────────
    /// <summary>Oyun başladığında (StartScreenUI'dan sonra).</summary>
    public static event System.Action OnGameStarted;

    /// <summary>Player öldüğünde. Vector3 = player pozisyonu.</summary>
    public static event System.Action<UnityEngine.Vector3> OnPlayerDied;

    // ── Yayın Metotları ───────────────────────────────────────────────────────
    public static void RaisePlayerJumped(float chargeRatio)     => OnPlayerJumped?.Invoke(chargeRatio);
    public static void RaisePlayerLanded(float impactSpeed)     => OnPlayerLanded?.Invoke(impactSpeed);
    public static void RaiseSpringBounced()                     => OnSpringBounced?.Invoke();
    public static void RaisePlatformBroken(UnityEngine.Vector3 pos) => OnPlatformBroken?.Invoke(pos);
    public static void RaiseSizeShifted(float multiplier)       => OnSizeShifted?.Invoke(multiplier);
    public static void RaiseGameStarted()                       => OnGameStarted?.Invoke();
    public static void RaisePlayerDied(UnityEngine.Vector3 pos) => OnPlayerDied?.Invoke(pos);
}
