using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Idle, Playing, Dead }

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraFollow cameraFollow;

    public PlayerController Player => player;
    public GameState State { get; private set; } = GameState.Idle;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        EnsureReferences();
    }

    private void Start()
    {
        EnsureReferences();
    }

    public void StartGame()
    {
        TryStartGame();
    }

    public bool TryStartGame()
    {
        EnsureReferences();
        if (State == GameState.Playing || player == null) return false;
        if (EnergyManager.Instance != null && !EnergyManager.Instance.ConsumeEnergy()) return false;

        State = GameState.Playing;
        ScoreManager.Instance?.StartTracking(player.transform);
        cameraFollow?.StartFollowing(player.transform);
        GameEvents.RaiseGameStarted();
        AudioManager.Instance?.PlayMusic();
        return true;
    }

    public void TriggerGameOver()
    {
        if (State != GameState.Playing) return;

        State = GameState.Dead;
        Vector3 deathPosition = player != null ? player.transform.position : Vector3.zero;
        GameEvents.RaisePlayerDied(deathPosition);
        ScoreManager.Instance?.EndGame();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void RevivePlayer()
    {
        EnsureReferences();
        if (State != GameState.Dead || player == null) return;

        State = GameState.Playing;
        player.transform.position += Vector3.up * 5f;
        player.ActivateShield();
        ScoreManager.Instance?.ResumeTracking(player.transform);
        GameEvents.RaiseGameStarted();
    }

    private void EnsureReferences()
    {
        if (player == null) player = FindAnyObjectByType<PlayerController>();

        if (cameraFollow == null)
        {
            cameraFollow = FindAnyObjectByType<CameraFollow>();
            if (cameraFollow == null && Camera.main != null)
                cameraFollow = Camera.main.GetComponent<CameraFollow>() ?? Camera.main.gameObject.AddComponent<CameraFollow>();
        }
    }
}
