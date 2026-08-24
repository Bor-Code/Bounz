using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10000)]
public class GameBootstrapper : MonoBehaviour
{
    private static bool _isBootstrapped;
    private static GameBootstrapper _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateBeforeSceneLoad()
    {
        if (_isBootstrapped || _instance != null) return;

        GameObject go = new GameObject("GameBootstrapper");
        go.AddComponent<GameBootstrapper>();
        DontDestroyOnLoad(go);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _isBootstrapped = true;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        BootstrapProject();
    }

    private void OnDestroy()
    {
        if (_instance != this) return;

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        _instance = null;
        _isBootstrapped = false;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        BootstrapProject();
    }

    private void BootstrapProject()
    {
        EnsureManager<SaveManager>("SaveManager");
        EnsureManager<AudioManager>("AudioManager");
        EnsureManager<HapticManager>("HapticManager");

        SettingsManager settings = EnsureManager<SettingsManager>("SettingsManager");
        settings.ApplySettings();

        EnsureManager<AnalyticsManager>("AnalyticsManager");
        EnsureManager<IAPManager>("IAPManager");
        EnsureManager<AdManager>("AdManager");
        EnsureManager<NotificationManager>("NotificationManager");
        EnsureManager<SkinManager>("SkinManager");
        EnsureManager<EnergyManager>("EnergyManager");
        EnsureManager<UpgradeManager>("UpgradeManager");
        EnsureManager<VIPManager>("VIPManager");
        EnsureManager<BattlePassManager>("BattlePassManager");
        EnsureManager<ChestManager>("ChestManager");
        EnsureManager<DailyRewardManager>("DailyRewardManager");
        EnsureManager<DailySpinManager>("DailySpinManager");
        EnsureManager<OfflineEarningsManager>("OfflineEarningsManager");
        EnsureManager<ProfileManager>("ProfileManager");
        EnsureManager<PromoCodeManager>("PromoCodeManager");
        EnsureManager<TournamentManager>("TournamentManager");
        EnsureManager<PiggyBankManager>("PiggyBankManager");
        EnsureManager<QuestManager>("QuestManager");
        EnsureManager<AchievementManager>("AchievementManager");
        EnsureManager<TutorialManager>("TutorialManager");

        EnsureCamera();
        EnsurePlayer();
        EnsurePlatformPool();
        EnsureManager<ScoreManager>("ScoreManager");
        EnsureManager<GameManager>("GameManager");
        EnsurePlatformSpawner();
        EnsureEventSystem();
        EnsureRuntimeUI();
    }

    private T EnsureManager<T>(string objectName) where T : Component
    {
        T existing = FindAnyObjectByType<T>();
        if (existing != null) return existing;

        GameObject go = new GameObject(objectName);
        return go.AddComponent<T>();
    }

    private void EnsureCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject go = new GameObject("Main Camera");
            try { go.tag = "MainCamera"; } catch { }
            cam = go.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            go.transform.position = new Vector3(0f, 2f, -10f);
        }

        if (cam.GetComponent<CameraFollow>() == null)
        {
            cam.gameObject.AddComponent<CameraFollow>();
        }
    }

    private void EnsurePlayer()
    {
        if (FindAnyObjectByType<PlayerController>() != null) return;

        GameObject player = new GameObject("Player");
        try { player.tag = "Player"; } catch { }
        player.transform.position = new Vector3(0f, 0f, 0f);

        SpriteRenderer visual = player.AddComponent<SpriteRenderer>();
        visual.sprite = CreateRuntimeSprite();
        visual.color = Color.white;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2.5f;
        rb.freezeRotation = true;

        CircleCollider2D bodyCollider = player.AddComponent<CircleCollider2D>();
        bodyCollider.radius = 0.45f;

        player.AddComponent<InputHandler>();

        GameObject groundCheck = new GameObject("GroundDetector");
        groundCheck.transform.SetParent(player.transform);
        groundCheck.transform.localPosition = new Vector3(0f, -0.55f, 0f);
        CircleCollider2D groundTrigger = groundCheck.AddComponent<CircleCollider2D>();
        groundTrigger.radius = 0.2f;
        groundTrigger.isTrigger = true;
        groundCheck.AddComponent<GroundDetector>();

        player.AddComponent<PlayerController>();
        player.AddComponent<FallDetector>();
    }

    private void EnsurePlatformPool()
    {
        EnsureManager<PlatformPool>("PlatformPool");
    }

    private void EnsurePlatformSpawner()
    {
        PlatformSpawner existing = FindAnyObjectByType<PlatformSpawner>();
        if (existing != null) return;

        GameObject go = new GameObject("PlatformSpawner");
        go.AddComponent<PlatformSpawner>();
    }

    private void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;

        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    private void EnsureRuntimeUI()
    {
        if (FindAnyObjectByType<UIManager>() != null || FindAnyObjectByType<RuntimeGameUI>() != null) return;

        GameObject go = new GameObject("RuntimeGameUI");
        go.AddComponent<RuntimeGameUI>();
    }

    public static Sprite CreateRuntimeSprite()
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
