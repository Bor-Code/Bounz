using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text coinText;
    [Header("Game Over Panel")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameObject newRecordBadge;
    [SerializeField] private Button restartButton;
    [Header("Animation")]
    [Tooltip("Panel fade-in + scale-up animasyonunun süresi (saniye)")]
    [SerializeField] private float panelAnimDuration = 0.4f;
    private void OnEnable()
    {
        ScoreEvents.OnScoreChanged += HandleScoreChanged;
        ScoreEvents.OnGameOver     += HandleGameOver;
        GameEvents.OnCoinCollected += HandleCoinCollected;
    }
    private void OnDisable()
    {
        ScoreEvents.OnScoreChanged -= HandleScoreChanged;
        ScoreEvents.OnGameOver     -= HandleGameOver;
        GameEvents.OnCoinCollected -= HandleCoinCollected;
    }
    [SerializeField] private Button reviveButton;
    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (newRecordBadge != null) newRecordBadge.SetActive(false);
        if (scoreText != null) scoreText.text = "0";
        UpdateCoinUI();
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
        if (reviveButton != null)
            reviveButton.onClick.AddListener(OnReviveClicked);
    }
    private void HandleScoreChanged(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
    private void HandleCoinCollected(Vector3 _)
    {
        UpdateCoinUI();
    }
    private void UpdateCoinUI()
    {
        if (coinText != null && SkinManager.Instance != null)
        {
            coinText.text = SkinManager.Instance.GetTotalScore().ToString();
        }
    }
    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        if (finalScoreText != null)
            finalScoreText.text = finalScore.ToString();
        if (highScoreText != null && ScoreManager.Instance != null)
            highScoreText.text = ScoreManager.Instance.HighScore.ToString();
        if (newRecordBadge != null)
            newRecordBadge.SetActive(isNewHighScore);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (reviveButton != null)
                reviveButton.gameObject.SetActive(true);
            StartCoroutine(AnimatePanel());
        }
    }
    private IEnumerator AnimatePanel()
    {
        if (panelCanvasGroup == null) yield break;
        panelCanvasGroup.alpha = 0f;
        gameOverPanel.transform.localScale = Vector3.one * 0.75f;
        float elapsed = 0f;
        while (elapsed < panelAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime;   
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelAnimDuration);
            panelCanvasGroup.alpha            = t;
            gameOverPanel.transform.localScale = Vector3.LerpUnclamped(
                Vector3.one * 0.75f, Vector3.one, t);
            yield return null;
        }
        panelCanvasGroup.alpha            = 1f;
        gameOverPanel.transform.localScale = Vector3.one;
    }
    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
    private void OnReviveClicked()
    {
        if (AdManager.Instance != null)
        {
            AdManager.Instance.ShowRewardedAd(
                onSuccess: () => 
                {
                    if (reviveButton != null) reviveButton.gameObject.SetActive(false);
                    if (gameOverPanel != null) gameOverPanel.SetActive(false);
                    if (GameManager.Instance != null) GameManager.Instance.RevivePlayer();
                },
                onFailed: () => 
                {
                    // Reklam yüklenemedi uyarısı verilebilir.
                }
            );
        }
        else
        {
            if (reviveButton != null) reviveButton.gameObject.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (GameManager.Instance != null) GameManager.Instance.RevivePlayer();
        }
    }
}