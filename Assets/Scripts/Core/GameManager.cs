using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Idle, Playing, Dead }

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private CameraFollow cameraFollow;

    public GameState State { get; private set; } = GameState.Idle;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // Oyun başlatmayı StartScreenUI'ya bırakıyoruz.
        // StartGame() sahneye yerleştirilmiş StartScreenUI tarafından çağrılır.
    }

    public void StartGame()
    {
        if (State == GameState.Playing) return;
        State = GameState.Playing;
        ScoreManager.Instance.StartTracking(player.transform);
        cameraFollow?.StartFollowing(player.transform);
        GameEvents.RaiseGameStarted();
        AudioManager.Instance?.PlayMusic();
    }

    public void TriggerGameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.Dead;
        GameEvents.RaisePlayerDied(player.transform.position);
        ScoreManager.Instance.EndGame();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
