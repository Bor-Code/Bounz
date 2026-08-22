using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Player basılı tutarken dolum çubuğunu ve renk geçişini günceller.
/// Ayrıca Player'ın etrafında ufak bir "glow halka" scale animasyonu tetikler.
///
/// Canvas > HUD altına ekle; alanları Inspector'dan bağla.
/// </summary>
public class ChargeIndicator : MonoBehaviour
{
    [Header("Referanslar")]
    [SerializeField] private PlayerController player;

    [Header("Fill Bar")]
    [Tooltip("Charge miktarını gösteren Image (Image Type: Filled).")]
    [SerializeField] private Image fillBar;

    [Tooltip("Dolum çubuğunun arka planı (opsiyonel, her zaman görünür).")]
    [SerializeField] private GameObject barBackground;

    [Header("Renk Geçişi")]
    [Tooltip("Düşük şarj rengi (örn. yeşil).")]
    [SerializeField] private Color colorLow  = new Color(0.3f, 0.95f, 0.4f);

    [Tooltip("Yüksek şarj rengi (örn. turuncu-kırmızı).")]
    [SerializeField] private Color colorHigh = new Color(1f, 0.35f, 0.15f);

    [Header("Glow Halkası (opsiyonel)")]
    [Tooltip("Player'ın üzerindeki halka Transform (Camera Space değil, World Space olmalı).")]
    [SerializeField] private Transform glowRing;

    [Tooltip("Halkanın min ve max scale'i.")]
    [SerializeField] private Vector2 glowScaleRange = new Vector2(0.8f, 1.6f);

    [Header("Titreme (Tam Şarj)")]
    [Tooltip("Tam şarjda UI'ın titreyip titremeyeceği.")]
    [SerializeField] private bool shakeOnFullCharge = true;
    [SerializeField] private float shakeAmplitude = 4f;
    [SerializeField] private float shakeFrequency = 30f;

    private RectTransform _fillBarRect;
    private Vector2 _barOriginalPos;
    private bool _wasCharging = false;

    private void Awake()
    {
        if (fillBar != null)
            _fillBarRect = fillBar.GetComponent<RectTransform>();
        if (_fillBarRect != null)
            _barOriginalPos = _fillBarRect.anchoredPosition;
    }

    private void Start()
    {
        SetVisible(false);
    }

    private void Update()
    {
        if (player == null) return;

        bool isCharging = player.IsCharging;
        float ratio     = player.ChargeRatio;

        // Görünürlük
        if (isCharging != _wasCharging)
        {
            SetVisible(isCharging);
            _wasCharging = isCharging;
        }

        if (!isCharging && ratio <= 0f) return;

        // Fill bar
        if (fillBar != null)
        {
            fillBar.fillAmount = ratio;
            fillBar.color      = Color.Lerp(colorLow, colorHigh, ratio);
        }

        // Glow halka
        if (glowRing != null)
        {
            float scale = Mathf.Lerp(glowScaleRange.x, glowScaleRange.y, ratio);
            glowRing.localScale = Vector3.one * scale;
        }

        // Titreme — tam şarjda
        if (shakeOnFullCharge && _fillBarRect != null && ratio >= 0.99f)
        {
            float offsetX = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
            float offsetY = Mathf.Cos(Time.time * shakeFrequency * 1.3f) * shakeAmplitude * 0.5f;
            _fillBarRect.anchoredPosition = _barOriginalPos + new Vector2(offsetX, offsetY);
        }
        else if (_fillBarRect != null)
        {
            _fillBarRect.anchoredPosition = _barOriginalPos;
        }
    }

    private void SetVisible(bool visible)
    {
        if (fillBar != null)       fillBar.gameObject.SetActive(visible);
        if (barBackground != null) barBackground.SetActive(visible);
        if (glowRing != null)      glowRing.gameObject.SetActive(visible);

        // Dolumu sıfırla kapanınca
        if (!visible && fillBar != null)
        {
            fillBar.fillAmount = 0f;
            if (_fillBarRect != null)
                _fillBarRect.anchoredPosition = _barOriginalPos;
        }
    }
}
