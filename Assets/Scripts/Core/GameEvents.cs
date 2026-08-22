public static class GameEvents
{
    public static event System.Action<float> OnPlayerJumped;
    public static event System.Action<float> OnPlayerLanded;
    public static event System.Action OnSpringBounced;
    public static event System.Action<UnityEngine.Vector3> OnPlatformBroken;
    public static event System.Action<float> OnSizeShifted;
    public static event System.Action OnGameStarted;
    public static event System.Action<UnityEngine.Vector3> OnPlayerDied;
    public static event System.Action<UnityEngine.Vector3> OnCoinCollected;
    public static event System.Action<int, UnityEngine.Vector3> OnPerfectLanding;
    public static event System.Action OnComboBroken;
    public static void RaisePlayerJumped(float chargeRatio)     => OnPlayerJumped?.Invoke(chargeRatio);
    public static void RaisePlayerLanded(float impactSpeed)     => OnPlayerLanded?.Invoke(impactSpeed);
    public static void RaiseSpringBounced()                     => OnSpringBounced?.Invoke();
    public static void RaisePlatformBroken(UnityEngine.Vector3 pos) => OnPlatformBroken?.Invoke(pos);
    public static void RaiseSizeShifted(float multiplier)       => OnSizeShifted?.Invoke(multiplier);
    public static void RaiseGameStarted()                       => OnGameStarted?.Invoke();
    public static void RaisePlayerDied(UnityEngine.Vector3 pos) => OnPlayerDied?.Invoke(pos);
    public static void RaiseCoinCollected(UnityEngine.Vector3 pos) => OnCoinCollected?.Invoke(pos);
    public static void RaisePerfectLanding(int combo, UnityEngine.Vector3 pos) => OnPerfectLanding?.Invoke(combo, pos);
    public static void RaiseComboBroken() => OnComboBroken?.Invoke();
}