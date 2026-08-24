

using UnityEngine;

using UnityEngine.EventSystems;



[DefaultExecutionOrder(-10000)]

public class GameBootstrapper : MonoBehaviour

{

    private static bool _isBootstrapped;



    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    private static void CreateBeforeSceneLoad()

    {

        if (_isBootstrapped) return;

        GameObject go = new GameObject("GameBootstrapper");

        go.AddComponent<GameBootstrapper>();

        DontDestroyOnLoad(go);

    }



    private void Awake()

    {

        if (_isBootstrapped && FindObjectsByType<GameBootstrapper>(FindObjectsSortMode.None).Length > 1)

        {

            Destroy(gameObject);

            return;

        }



        _isBootstrapped = true;

        DontDestroyOnLoad(gameObject);

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

    }



    private T EnsureManager<T>(string objectName) where T : Component

    {

        T existing = FindFirstObjectByType<T>();

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

        if (FindFirstObjectByType<PlayerController>() != null) return;



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

        PlatformSpawner existing = FindFirstObjectByType<PlatformSpawner>();

        if (existing != null) return;



        GameObject go = new GameObject("PlatformSpawner");

        go.AddComponent<PlatformSpawner>();

    }



    private void EnsureEventSystem()

    {

        if (FindFirstObjectByType<EventSystem>() != null) return;



        GameObject go = new GameObject("EventSystem");

        go.AddComponent<EventSystem>();

        go.AddComponent<StandaloneInputModule>();

    }



    public static Sprite CreateRuntimeSprite()

    {

        Texture2D texture = new Texture2D(1, 1);

        texture.SetPixel(0, 0, Color.white);

        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);

    }

}

