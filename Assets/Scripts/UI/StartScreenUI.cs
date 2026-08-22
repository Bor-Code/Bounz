using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Oyunun başlangıç ekranını yönetir.
/// "Tap to Start" a basılınca GameManager.StartGame()'i tetikler
/// ve kendi panelini kapatır.
///
/// Hierarchy: Canvas > StartPanel > bu script'i içeren GameObject'e ekle.
/// Ya da Canvas'a ekleyip alanları Inspector'dan bağla.
/// </summary>
public class StartScreenUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private CanvasGroup panelCanvasGroup;

    [Header("UI Elemanları")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tapPromptText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Animasyon")]
    [Tooltip("Tap prompt'un soluk alıp verme hızı (saniye).")]
    [SerializeField] private float pulseSpeed = 1.2f;

    [Tooltip("Başlangıç panelinin kaybolma süresi.")]
    [SerializeField] private float fadeOutDuration = 0.3f;

    private bool _gameStarted = false;

    private void Start()
    {
        // Başlangıçta game manager'ı Idle'da tut (Start() içinde StartGame çağrılmasın)
        // GameManager.Start()'ta StartGame() çağrısı var — bunu önlemek için
        // GameManager'a Idle durumunu ekledik, ama mevcut kodda Start() direkt StartGame() çağırıyor.
        // Bu script o çağrıyı override etmez; GameManager'ı el ile Idle'a almak gerekiyor.
        // Bkz. kurulum kılavuzu Adım 4.

        // Best score yükle
        if (bestScoreText != null)
        {
            int best = PlayerPrefs.GetInt("HighScore", 0);
            bestScoreText.text = best > 0 ? $"BEST: {best}" : "";
        }

        // Pulse animasyonunu başlat
        StartCoroutine(PulsePrompt());
    }

    private void Update()
    {
        if (_gameStarted) return;

        // Herhangi bir dokunuş veya mouse tıklaması
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            BeginGame();
        }
    }

    private void BeginGame()
    {
        _gameStarted = true;
        StartCoroutine(FadeOutAndStart());
    }

    private IEnumerator FadeOutAndStart()
    {
        // Panel yavaşça kaybolur
        float elapsed = 0f;
        float startAlpha = panelCanvasGroup != null ? panelCanvasGroup.alpha : 1f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeOutDuration;
            if (panelCanvasGroup != null)
                panelCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);
            yield return null;
        }

        if (startPanel != null) startPanel.SetActive(false);

        // Oyunu başlat
        GameManager.Instance?.StartGame();
    }

    private IEnumerator PulsePrompt()
    {
        while (!_gameStarted)
        {
            // 0.4 → 1.0 → 0.4 arası alpha soluk alıp verme
            float t = (Mathf.Sin(Time.time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
            float alpha = Mathf.Lerp(0.4f, 1f, t);

            if (tapPromptText != null)
            {
                Color c = tapPromptText.color;
                c.a = alpha;
                tapPromptText.color = c;
            }

            yield return null;
        }
    }
}
