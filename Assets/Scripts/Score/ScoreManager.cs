using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";
    public static ScoreManager Instance { get; private set; }
    [Header("Scoring")]
    [SerializeField] private float distanceToPointRatio = 1f;
    private int _score;
    private int _highScore;
    private bool _isActive;
    private float _startX;
    private Transform _playerTransform;
    public int Score => _score;
    public int HighScore => _highScore;
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (SaveManager.Instance != null)
        {
            _highScore = SaveManager.Instance.CurrentSave.highScore;
        }
    }
    private float _lastX;
    private float _floatScore;
    public float Multiplier { get; set; } = 1f;
    public void StartTracking(Transform player)
    {
        _playerTransform = player;
        _startX = player.position.x;
        _lastX = player.position.x;
        _score = 0;
        _floatScore = 0f;
        Multiplier = 1f;
        _isActive = true;
        ScoreEvents.RaiseScoreChanged(_score);
    }
    private void Update()
    {
        if (!_isActive || _playerTransform == null) return;
        float deltaX = _playerTransform.position.x - _lastX;
        if (deltaX > 0)
        {
            _floatScore += deltaX * distanceToPointRatio * Multiplier;
            _lastX = _playerTransform.position.x;
        }
        int newScore = Mathf.FloorToInt(_floatScore);
        if (newScore != _score)
        {
            _score = newScore;
            ScoreEvents.RaiseScoreChanged(_score);
        }
    }
    public void AddComboScore(int amount)
    {
        if (!_isActive) return;
        _floatScore += amount;
        int newScore = Mathf.FloorToInt(_floatScore);
        if (newScore != _score)
        {
            _score = newScore;
            ScoreEvents.RaiseScoreChanged(_score);
        }
    }
    public void EndGame()
    {
        if (!_isActive) return;
        _isActive = false;
        bool isNewHighScore = _score > _highScore;
        if (isNewHighScore)
        {
            _highScore = _score;
            if (SaveManager.Instance != null)
            {
                SaveManager.Instance.CurrentSave.highScore = _highScore;
                SaveManager.Instance.SaveGame();
            }
        }
        ScoreEvents.RaiseGameOver(_score, isNewHighScore);
    }
    public void ResetHighScore()
    {
        _highScore = 0;
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.highScore = 0;
            SaveManager.Instance.SaveGame();
        }
    }
}