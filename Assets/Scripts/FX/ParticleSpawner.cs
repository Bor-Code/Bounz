using UnityEngine;

/// <summary>
/// GameEvents event'lerini dinleyerek uygun konumda particle efekti oynatır.
/// Her efekt tipi için ayrı bir ParticleSystem prefab bağlanır (Inspector).
///
/// Sahneye boş bir GameObject ekle → ParticleSpawner bileşenini ata.
/// Prefab alanlarına Particle System prefab'larını bağla.
/// </summary>
public class ParticleSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [Tooltip("Player zıplarken zeminde çıkan toz bulutu.")]
    [SerializeField] private ParticleSystem jumpDustPrefab;

    [Tooltip("Player platforma inince çıkan toz bulutu.")]
    [SerializeField] private ParticleSystem landDustPrefab;

    [Tooltip("Spring platform efekti.")]
    [SerializeField] private ParticleSystem springBurstPrefab;

    [Tooltip("Fragile platform parçalandığında çıkan kıvılcım/parça.")]
    [SerializeField] private ParticleSystem breakPrefab;

    [Tooltip("SizeShift tetiklenince çıkan halka efekti.")]
    [SerializeField] private ParticleSystem sizeShiftRingPrefab;

    [Tooltip("Player öldüğünde çıkan büyük patlama efekti.")]
    [SerializeField] private ParticleSystem deathBurstPrefab;

    // Player referansı — iniş/zıplama efekti player'ın konumunda oynar
    [Header("Reference")]
    [SerializeField] private Transform playerTransform;

    private void OnEnable()
    {
        GameEvents.OnPlayerJumped   += HandleJump;
        GameEvents.OnPlayerLanded   += HandleLand;
        GameEvents.OnSpringBounced  += HandleSpring;
        GameEvents.OnPlatformBroken += HandleBreak;
        GameEvents.OnSizeShifted    += HandleSizeShift;
        GameEvents.OnPlayerDied     += HandleDeath;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerJumped   -= HandleJump;
        GameEvents.OnPlayerLanded   -= HandleLand;
        GameEvents.OnSpringBounced  -= HandleSpring;
        GameEvents.OnPlatformBroken -= HandleBreak;
        GameEvents.OnSizeShifted    -= HandleSizeShift;
        GameEvents.OnPlayerDied     -= HandleDeath;
    }

    // ── Handler'lar ───────────────────────────────────────────────────────────

    private void HandleJump(float _)
    {
        if (playerTransform != null)
            Spawn(jumpDustPrefab, playerTransform.position);
    }

    private void HandleLand(float impactSpeed)
    {
        if (playerTransform == null) return;
        var ps = Spawn(landDustPrefab, playerTransform.position);
        if (ps != null)
        {
            // Hıza göre emission sayısını ölçekle
            var emission = ps.emission;
            var burst = emission.GetBurst(0);
            burst.count = new ParticleSystem.MinMaxCurve(
                Mathf.Lerp(5f, 20f, Mathf.InverseLerp(2f, 12f, impactSpeed)));
            emission.SetBurst(0, burst);
            ps.Play();
        }
    }

    private void HandleSpring()
    {
        if (playerTransform != null)
            Spawn(springBurstPrefab, playerTransform.position);
    }

    private void HandleBreak(Vector3 pos)  => Spawn(breakPrefab, pos);
    private void HandleSizeShift(float _)
    {
        if (playerTransform != null)
            Spawn(sizeShiftRingPrefab, playerTransform.position);
    }

    private void HandleDeath(Vector3 pos)  => Spawn(deathBurstPrefab, pos);

    // ── Yardımcı ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Prefab'dan bir klon oluşturur, oynatır ve bitince yok eder.
    /// </summary>
    private ParticleSystem Spawn(ParticleSystem prefab, Vector3 position)
    {
        if (prefab == null) return null;

        ParticleSystem instance = Instantiate(prefab, position, Quaternion.identity);
        instance.Play();
        Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        return instance;
    }
}
