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
    }
    private void Start()
    {
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
    public void RevivePlayer()
    {
        if (State != GameState.Dead) return;
        State = GameState.Playing;
        
        // Oyuncuyu havaya ışınlayalım ve kalkan verelim
        player.transform.position += Vector3.up * 5f;
        player.ActivateShield();
        
        // Skoru kaldığı yerden devam ettir
        ScoreManager.Instance.ResumeTracking(player.transform);
        GameEvents.RaiseGameStarted(); // Oyunu tekrar başlatma event'i (müzik vb)
    }
}