using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [Header("Jump")]
    [SerializeField] private AudioClip jumpLow;
    [SerializeField] private AudioClip jumpHigh;
    [SerializeField] private AudioClip perfectLanding;
    [Header("Land")]
    [SerializeField] private AudioClip landLight;
    [SerializeField] private AudioClip landHeavy;
    [Header("Platform")]
    [SerializeField] private AudioClip springBounce;
    [SerializeField] private AudioClip platformBreak;
    [SerializeField] private AudioClip sizeShift;
    [Header("Game State")]
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip gameStartSfx;
    [SerializeField] private AudioClip coinCollectSfx;
    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [Tooltip("Arka plan müziği ses seviyesi.")]
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 0.35f;
    [Header("SFX Volume")]
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 0.85f;
    private AudioSource _musicSource;
    private AudioSource _sfxSource;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop   = true;
        _musicSource.volume = musicVolume;
        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerJumped    += HandleJump;
        GameEvents.OnPlayerLanded    += HandleLand;
        GameEvents.OnSpringBounced   += HandleSpring;
        GameEvents.OnPlatformBroken  += HandlePlatformBreak;
        GameEvents.OnSizeShifted     += HandleSizeShift;
        GameEvents.OnGameStarted     += HandleGameStarted;
        GameEvents.OnPlayerDied      += HandlePlayerDied;
        GameEvents.OnCoinCollected   += HandleCoinCollected;
        GameEvents.OnPerfectLanding  += HandlePerfectLanding;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerJumped    -= HandleJump;
        GameEvents.OnPlayerLanded    -= HandleLand;
        GameEvents.OnSpringBounced   -= HandleSpring;
        GameEvents.OnPlatformBroken  -= HandlePlatformBreak;
        GameEvents.OnSizeShifted     -= HandleSizeShift;
        GameEvents.OnGameStarted     -= HandleGameStarted;
        GameEvents.OnPlayerDied      -= HandlePlayerDied;
        GameEvents.OnCoinCollected   -= HandleCoinCollected;
        GameEvents.OnPerfectLanding  -= HandlePerfectLanding;
    }
    private void HandleJump(float chargeRatio)
    {
        AudioClip clip = chargeRatio > 0.5f ? jumpHigh : jumpLow;
        PlaySfx(clip, pitch: Mathf.Lerp(0.9f, 1.2f, chargeRatio));
    }
    private void HandleLand(float impactSpeed)
    {
        AudioClip clip = impactSpeed > 6f ? landHeavy : landLight;
        PlaySfx(clip, pitch: Mathf.Lerp(0.95f, 1.1f, Mathf.InverseLerp(2f, 12f, impactSpeed)));
    }
    private void HandleSpring()      => PlaySfx(springBounce, pitch: 1.1f);
    private void HandleSizeShift(float _) => PlaySfx(sizeShift);
    private void HandleGameStarted() => PlaySfx(gameStartSfx);
    private void HandleCoinCollected(Vector3 _) => PlaySfx(coinCollectSfx);
    private void HandlePerfectLanding(int combo, Vector3 _)
    {
        float pitch = Mathf.Clamp(1f + (combo * 0.1f), 1f, 2f);
        PlaySfx(perfectLanding, pitch);
    }
    private void HandlePlatformBreak(Vector3 _) => PlaySfx(platformBreak);
    private void HandlePlayerDied(Vector3 _)
    {
        _musicSource.Stop();
        PlaySfx(gameOverSfx);
    }
    private void PlaySfx(AudioClip clip, float pitch = 1f)
    {
        if (clip == null) return;
        _sfxSource.pitch = pitch;
        _sfxSource.PlayOneShot(clip, sfxVolume);
    }
    public void PlayMusic()
    {
        if (backgroundMusic == null || _musicSource.isPlaying) return;
        _musicSource.clip = backgroundMusic;
        _musicSource.Play();
    }
    public void StopMusic() => _musicSource.Stop();
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (_musicSource != null) _musicSource.volume = volume;
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}