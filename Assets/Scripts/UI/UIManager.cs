using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Oyunun UI katmanını yönetir:
///   - HUD: Anlık skor gösterimi
///   - Game Over Panel: Final skor, high score, new record badge, restart butonu
///
/// Canvas > UIManager GameObject'e ekle, Inspector'dan alanları bağla.
/// </summary>
public class UIManager : MonoBehaviour
{
    // ── HUD ──────────────────────────────────────────────────────────────────
    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;

    // ── Game Over Panel ───────────────────────────────────────────────────────
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

    // ─────────────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        ScoreEvents.OnScoreChanged += HandleScoreChanged;
        ScoreEvents.OnGameOver     += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnScoreChanged -= HandleScoreChanged;
        ScoreEvents.OnGameOver     -= HandleGameOver;
    }

    private void Start()
    {
        // Başlangıçta panel kapalı
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (newRecordBadge != null) newRecordBadge.SetActive(false);
        if (scoreText != null) scoreText.text = "0";

        // Restart butonu bağlantısı
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void HandleScoreChanged(int score)
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
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
            StartCoroutine(AnimatePanel());
        }
    }

    // ── Animasyon ─────────────────────────────────────────────────────────────

    private IEnumerator AnimatePanel()
    {
        if (panelCanvasGroup == null) yield break;

        // Başlangıç durumu: tamamen şeffaf + küçük
        panelCanvasGroup.alpha = 0f;
        gameOverPanel.transform.localScale = Vector3.one * 0.75f;

        float elapsed = 0f;
        while (elapsed < panelAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime;   // Time.timeScale = 0 olsa bile çalışır
            float t = Mathf.SmoothStep(0f, 1f, elapsed / panelAnimDuration);

            panelCanvasGroup.alpha            = t;
            gameOverPanel.transform.localScale = Vector3.LerpUnclamped(
                Vector3.one * 0.75f, Vector3.one, t);

            yield return null;
        }

        panelCanvasGroup.alpha            = 1f;
        gameOverPanel.transform.localScale = Vector3.one;
    }

    // ── Buton ─────────────────────────────────────────────────────────────────

    private void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }
}
