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
        _highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    public void StartTracking(Transform player)
    {
        _playerTransform = player;
        _startX = player.position.x;
        _score = 0;
        _isActive = true;
        ScoreEvents.RaiseScoreChanged(_score);
    }

    private void Update()
    {
        if (!_isActive || _playerTransform == null) return;

        int newScore = Mathf.Max(0, Mathf.FloorToInt((_playerTransform.position.x - _startX) * distanceToPointRatio));
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
            PlayerPrefs.SetInt(HighScoreKey, _highScore);
            PlayerPrefs.Save();
        }

        if (SkinManager.Instance != null)
        {
            SkinManager.Instance.AddTotalScore(_score);
        }

        ScoreEvents.RaiseGameOver(_score, isNewHighScore);
    }

    public void ResetHighScore()
    {
        _highScore = 0;
        PlayerPrefs.DeleteKey(HighScoreKey);
    }
}
