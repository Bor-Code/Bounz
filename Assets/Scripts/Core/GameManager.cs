using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Idle, Playing, Dead }

    [Header("References")]
    [SerializeField] private PlayerController player;

    public GameState State { get; private set; } = GameState.Idle;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        State = GameState.Playing;
        ScoreManager.Instance.StartTracking(player.transform);
    }

    public void TriggerGameOver()
    {
        if (State != GameState.Playing) return;
        State = GameState.Dead;
        ScoreManager.Instance.EndGame();
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
