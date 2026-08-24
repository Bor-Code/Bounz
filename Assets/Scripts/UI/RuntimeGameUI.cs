using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuntimeGameUI : MonoBehaviour
{
    private Canvas _canvas;
    private GameObject _startPanel;
    private GameObject _hudPanel;
    private GameObject _gameOverPanel;
    private GameObject _outOfEnergyPanel;
    private TMP_Text _bestScoreText;
    private TMP_Text _startCoinText;
    private TMP_Text _startEnergyText;
    private TMP_Text _scoreText;
    private TMP_Text _coinText;
    private TMP_Text _energyText;
    private TMP_Text _energyTimerText;
    private TMP_Text _finalScoreText;
    private TMP_Text _highScoreText;
    private TMP_Text _newRecordText;
    private Button _reviveButton;

    private void Awake()
    {
        BuildUI();
    }

    private void OnEnable()
    {
        ScoreEvents.OnScoreChanged += HandleScoreChanged;
        ScoreEvents.OnGameOver += HandleGameOver;
        GameEvents.OnCoinCollected += HandleCoinCollected;
        if (EnergyManager.Instance != null) EnergyManager.Instance.OnEnergyUpdated += UpdateEnergyUI;
    }

    private void OnDisable()
    {
        ScoreEvents.OnScoreChanged -= HandleScoreChanged;
        ScoreEvents.OnGameOver -= HandleGameOver;
        GameEvents.OnCoinCollected -= HandleCoinCollected;
        if (EnergyManager.Instance != null) EnergyManager.Instance.OnEnergyUpdated -= UpdateEnergyUI;
    }

    private void Start()
    {
        RefreshAll();
    }

    private void Update()
    {
        if (_startPanel == null || !_startPanel.activeSelf) return;
        if (_outOfEnergyPanel != null && _outOfEnergyPanel.activeSelf) return;
        if (IsPointerOverUI()) return;

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            TryStartGame();
        }
    }

    private void BuildUI()
    {
        GameObject canvasObject = new GameObject("Runtime Canvas");
        canvasObject.transform.SetParent(transform);
        _canvas = canvasObject.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        _hudPanel = CreateRect("HUD", canvasObject.transform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(32f, -170f), new Vector2(-32f, -32f)).gameObject;
        _scoreText = CreateText("ScoreText", _hudPanel.transform, "0", 54, TextAlignmentOptions.TopLeft, new Vector2(0f, 0f), new Vector2(0.34f, 1f), Vector2.zero, Vector2.zero);
        _coinText = CreateText("CoinText", _hudPanel.transform, "Coins: 0", 34, TextAlignmentOptions.Top, new Vector2(0.33f, 0f), new Vector2(0.67f, 1f), Vector2.zero, Vector2.zero);
        _energyText = CreateText("EnergyText", _hudPanel.transform, "Energy: 0/0", 34, TextAlignmentOptions.TopRight, new Vector2(0.66f, 0f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
        _energyTimerText = CreateText("EnergyTimerText", _hudPanel.transform, "", 28, TextAlignmentOptions.TopRight, new Vector2(0.66f, 0f), new Vector2(1f, 1f), new Vector2(0f, -52f), new Vector2(0f, -52f));

        _startPanel = CreatePanel("StartPanel", canvasObject.transform, new Color(0.05f, 0.07f, 0.12f, 0.88f));
        CreateText("Title", _startPanel.transform, "BOUNZ", 96, TextAlignmentOptions.Center, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.78f), Vector2.zero, Vector2.zero);
        _bestScoreText = CreateText("BestScore", _startPanel.transform, "BEST: 0", 38, TextAlignmentOptions.Center, new Vector2(0.1f, 0.54f), new Vector2(0.9f, 0.61f), Vector2.zero, Vector2.zero);
        _startCoinText = CreateText("StartCoins", _startPanel.transform, "Coins: 0", 34, TextAlignmentOptions.Center, new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.54f), Vector2.zero, Vector2.zero);
        _startEnergyText = CreateText("StartEnergy", _startPanel.transform, "Energy: 0/0", 34, TextAlignmentOptions.Center, new Vector2(0.1f, 0.42f), new Vector2(0.9f, 0.48f), Vector2.zero, Vector2.zero);
        CreateButton("StartButton", _startPanel.transform, "START", new Vector2(0.24f, 0.29f), new Vector2(0.76f, 0.38f), TryStartGame);
        CreateText("TapPrompt", _startPanel.transform, "Tap anywhere to start", 30, TextAlignmentOptions.Center, new Vector2(0.1f, 0.21f), new Vector2(0.9f, 0.28f), Vector2.zero, Vector2.zero);

        _outOfEnergyPanel = CreatePanel("OutOfEnergyPanel", canvasObject.transform, new Color(0.08f, 0.05f, 0.08f, 0.94f));
        CreateText("OutOfEnergyTitle", _outOfEnergyPanel.transform, "ENERGY EMPTY", 58, TextAlignmentOptions.Center, new Vector2(0.1f, 0.58f), new Vector2(0.9f, 0.68f), Vector2.zero, Vector2.zero);
        CreateText("OutOfEnergyBody", _outOfEnergyPanel.transform, "Refill to keep playing.", 32, TextAlignmentOptions.Center, new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.57f), Vector2.zero, Vector2.zero);
        CreateButton("CoinRefillButton", _outOfEnergyPanel.transform, "REFILL WITH COINS", new Vector2(0.18f, 0.38f), new Vector2(0.82f, 0.47f), RefillEnergyWithCoins);
        CreateButton("AdRefillButton", _outOfEnergyPanel.transform, "WATCH AD REFILL", new Vector2(0.18f, 0.27f), new Vector2(0.82f, 0.36f), RefillEnergyWithAd);
        CreateButton("CloseOutOfEnergyButton", _outOfEnergyPanel.transform, "BACK", new Vector2(0.3f, 0.16f), new Vector2(0.7f, 0.24f), HideOutOfEnergy);
        _outOfEnergyPanel.SetActive(false);

        _gameOverPanel = CreatePanel("GameOverPanel", canvasObject.transform, new Color(0.08f, 0.05f, 0.08f, 0.92f));
        CreateText("GameOverTitle", _gameOverPanel.transform, "GAME OVER", 72, TextAlignmentOptions.Center, new Vector2(0.1f, 0.64f), new Vector2(0.9f, 0.75f), Vector2.zero, Vector2.zero);
        _finalScoreText = CreateText("FinalScore", _gameOverPanel.transform, "Score: 0", 42, TextAlignmentOptions.Center, new Vector2(0.1f, 0.56f), new Vector2(0.9f, 0.63f), Vector2.zero, Vector2.zero);
        _highScoreText = CreateText("HighScore", _gameOverPanel.transform, "Best: 0", 36, TextAlignmentOptions.Center, new Vector2(0.1f, 0.5f), new Vector2(0.9f, 0.56f), Vector2.zero, Vector2.zero);
        _newRecordText = CreateText("NewRecord", _gameOverPanel.transform, "NEW RECORD!", 34, TextAlignmentOptions.Center, new Vector2(0.1f, 0.44f), new Vector2(0.9f, 0.5f), Vector2.zero, Vector2.zero);
        CreateButton("RestartButton", _gameOverPanel.transform, "RESTART", new Vector2(0.2f, 0.32f), new Vector2(0.8f, 0.41f), RestartGame);
        _reviveButton = CreateButton("ReviveButton", _gameOverPanel.transform, "REVIVE WITH AD", new Vector2(0.2f, 0.21f), new Vector2(0.8f, 0.3f), ReviveWithAd);
        _gameOverPanel.SetActive(false);
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        RectTransform rect = CreateRect(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        return rect.gameObject;
    }

    private RectTransform CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        return rect;
    }

    private TMP_Text CreateText(string name, Transform parent, string text, float size, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, offsetMin, offsetMax);
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = size;
        label.alignment = alignment;
        label.color = Color.white;
        label.raycastTarget = false;
        return label;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 anchorMin, Vector2 anchorMax, UnityAction action)
    {
        RectTransform rect = CreateRect(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        Image image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.13f, 0.58f, 1f, 0.95f);
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        CreateText("Label", rect, label, 34, TextAlignmentOptions.Center, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        return button;
    }

    private void TryStartGame()
    {
        if (GameManager.Instance == null || !GameManager.Instance.TryStartGame())
        {
            ShowOutOfEnergy();
            return;
        }

        _startPanel.SetActive(false);
        _outOfEnergyPanel.SetActive(false);
        _hudPanel.SetActive(true);
        RefreshAll();
    }

    private void ShowOutOfEnergy()
    {
        if (_outOfEnergyPanel != null) _outOfEnergyPanel.SetActive(true);
        UpdateEnergyUI();
    }

    private void HideOutOfEnergy()
    {
        if (_outOfEnergyPanel != null) _outOfEnergyPanel.SetActive(false);
    }

    private void RefillEnergyWithCoins()
    {
        EnergyManager.Instance?.RefillEnergyWithCoins();
        RefreshAll();
        if (EnergyManager.Instance != null && EnergyManager.Instance.HasEnergy) HideOutOfEnergy();
    }

    private void RefillEnergyWithAd()
    {
        EnergyManager.Instance?.RefillEnergyWithAd();
        RefreshAll();
        if (EnergyManager.Instance != null && EnergyManager.Instance.HasEnergy) HideOutOfEnergy();
    }

    private void RestartGame()
    {
        GameManager.Instance?.RestartGame();
    }

    private void ReviveWithAd()
    {
        if (_reviveButton != null) _reviveButton.gameObject.SetActive(false);

        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd(
                onSuccess: () =>
                {
                    _gameOverPanel.SetActive(false);
                    GameManager.Instance?.RevivePlayer();
                },
                onFailed: () =>
                {
                    if (_reviveButton != null) _reviveButton.gameObject.SetActive(true);
                });
        }
        else
        {
            _gameOverPanel.SetActive(false);
            GameManager.Instance?.RevivePlayer();
        }
    }

    private void HandleScoreChanged(int score)
    {
        if (_scoreText != null) _scoreText.text = score.ToString();
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        if (_finalScoreText != null) _finalScoreText.text = $"Score: {finalScore}";
        if (_highScoreText != null) _highScoreText.text = $"Best: {GetHighScore()}";
        if (_newRecordText != null) _newRecordText.gameObject.SetActive(isNewHighScore);
        if (_reviveButton != null) _reviveButton.gameObject.SetActive(true);
        if (_gameOverPanel != null) _gameOverPanel.SetActive(true);
        RefreshAll();
    }

    private void HandleCoinCollected(Vector3 position)
    {
        UpdateCoinUI();
    }

    private void RefreshAll()
    {
        UpdateBestScoreUI();
        UpdateCoinUI();
        UpdateEnergyUI();
        HandleScoreChanged(ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0);
    }

    private void UpdateBestScoreUI()
    {
        int highScore = GetHighScore();
        if (_bestScoreText != null) _bestScoreText.text = highScore > 0 ? $"BEST: {highScore}" : "BEST: 0";
    }

    private void UpdateCoinUI()
    {
        int coins = GetCoins();
        if (_coinText != null) _coinText.text = $"Coins: {coins}";
        if (_startCoinText != null) _startCoinText.text = $"Coins: {coins}";
    }

    private void UpdateEnergyUI()
    {
        if (EnergyManager.Instance == null) return;

        string energy = $"Energy: {EnergyManager.Instance.CurrentEnergy}/{EnergyManager.Instance.MaxEnergy}";
        if (_energyText != null) _energyText.text = energy;
        if (_startEnergyText != null) _startEnergyText.text = energy;

        if (_energyTimerText != null)
        {
            if (EnergyManager.Instance.CurrentEnergy >= EnergyManager.Instance.MaxEnergy)
            {
                _energyTimerText.text = "Full";
            }
            else
            {
                var time = EnergyManager.Instance.TimeUntilNextEnergy;
                _energyTimerText.text = $"Next: {time.Minutes:D2}:{time.Seconds:D2}";
            }
        }
    }

    private int GetCoins()
    {
        if (SkinManager.Instance != null) return SkinManager.Instance.GetTotalScore();
        return SaveManager.Instance != null ? SaveManager.Instance.CurrentSave.totalScore : 0;
    }

    private int GetHighScore()
    {
        if (ScoreManager.Instance != null) return ScoreManager.Instance.HighScore;
        return SaveManager.Instance != null ? SaveManager.Instance.CurrentSave.highScore : 0;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        if (Input.touchCount > 0) return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return EventSystem.current.IsPointerOverGameObject();
    }
}
