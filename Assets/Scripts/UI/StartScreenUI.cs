using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartScreenUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private CanvasGroup panelCanvasGroup;
    [Header("UI Elemanları")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text tapPromptText;
    [SerializeField] private TMP_Text bestScoreText;
    [Header("Shop Integration")]
    [SerializeField] private Button shopButton;
    [SerializeField] private SkinStoreUI storeUI;
    private bool _isStoreOpen = false;
    [Header("Animasyon")]
    [Tooltip("Tap prompt'un soluk alıp verme hızı (saniye).")]
    [SerializeField] private float pulseSpeed = 1.2f;
    [Tooltip("Başlangıç panelinin kaybolma süresi.")]
    [SerializeField] private float fadeOutDuration = 0.3f;
    private bool _gameStarted = false;

    private void Start()
    {
        if (bestScoreText != null)
        {
            int best = SaveManager.Instance != null ? SaveManager.Instance.CurrentSave.highScore : SaveManager.GetIntValue("HighScore", 0);
            bestScoreText.text = best > 0 ? $"BEST: {best}" : "";
        }

        StartCoroutine(PulsePrompt());

        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
        }

        if (storeUI != null)
        {
            storeUI.OnStoreClosed += OnStoreClosed;
        }
    }

    private void OnDestroy()
    {
        if (storeUI != null)
        {
            storeUI.OnStoreClosed -= OnStoreClosed;
        }
    }

    private void OpenShop()
    {
        _isStoreOpen = true;
        if (storeUI != null) storeUI.OpenStore();
    }

    public void OnStoreClosed()
    {
        _isStoreOpen = false;
    }

    private void Update()
    {
        if (_gameStarted || _isStoreOpen) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }

            if (Input.touchCount == 0) return;
        }

        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            BeginGame();
        }
    }

    private void BeginGame()
    {
        if (GameManager.Instance == null || !GameManager.Instance.TryStartGame())
        {
            FindAnyObjectByType<EnergyUI>()?.ShowOutOfEnergyPanel();
            _gameStarted = false;
            return;
        }

        _gameStarted = true;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
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
    }

    private IEnumerator PulsePrompt()
    {
        while (!_gameStarted)
        {
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
