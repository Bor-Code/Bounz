using UnityEngine;
using System.Collections;

/// <summary>
/// Topun (Player) hızına göre esnemesini ve yere çarptığında basılmasını sağlar.
/// Sadece görsel bir efekttir, fizik (Collider) etkilenmemelidir.
///
/// KULLANIM (Çok Önemli):
/// 1. Player objesinin altındaki "SpriteRenderer"ı yeni bir Child objeye taşı (ör. "Visual").
/// 2. Bu scripti o Child objeye ("Visual") ekle!
/// 3. Inspector'da Player'ın ana objesindeki Rigidbody2D'yi referans olarak bağla.
/// </summary>
public class SquashAndStretch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player'ın ana objesindeki Rigidbody2D")]
    [SerializeField] private Rigidbody2D playerRb;

    [Header("Stretch Settings (Uçarken)")]
    [Tooltip("Hıza göre ne kadar esneyeceği (Y ekseni).")]
    [SerializeField] private float stretchMultiplier = 0.05f;
    [Tooltip("Maksimum esneme oranı.")]
    [SerializeField] private float maxStretch = 1.4f;

    [Header("Squash Settings (Çarparken)")]
    [Tooltip("Zemine çarpınca Y ekseninde ne kadar basılacak.")]
    [SerializeField] private float squashAmount = 0.5f;
    [Tooltip("Basıldıktan sonra orijinal haline dönme hızı.")]
    [SerializeField] private float squashRecoverySpeed = 10f;

    private Vector3 _originalScale;
    private bool _isSquashing = false;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerLanded += HandleLanding;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerLanded -= HandleLanding;
    }

    private void Update()
    {
        if (playerRb == null || _isSquashing) return;

        // Düşerken veya zıplarken Y eksenindeki hıza göre esneme hesapla
        float speedY = Mathf.Abs(playerRb.linearVelocity.y);
        
        // Hız sıfırsa veya çok küçükse esneme (zemin üzerindeyken falan titreşimi önler)
        if (speedY < 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, Time.deltaTime * squashRecoverySpeed);
            return;
        }

        float stretch = 1f + (speedY * stretchMultiplier);
        stretch = Mathf.Clamp(stretch, 1f, maxStretch);

        // Hacmi korumak için, Y uzuyorsa X kısalmalı (squash & stretch ilkesi)
        // Hacim = X * Y = 1, yani Y = stretch ise X = 1/stretch
        float squashX = 1f / stretch;

        // Rotasyonu hareket yönüne (vektöre) çevirmek istersen eklenebilir, 
        // ancak Bounz'da top dik zıpladığı için doğrudan localScale yeterlidir.
        Vector3 targetScale = new Vector3(_originalScale.x * squashX, _originalScale.y * stretch, _originalScale.z);

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 15f);
    }

    private void HandleLanding(float impactSpeed)
    {
        // Çok yavaş düştüyse efekt yapma
        if (impactSpeed < 2f) return;

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(SquashRoutine(impactSpeed));
        }
    }

    private IEnumerator SquashRoutine(float impactSpeed)
    {
        _isSquashing = true;

        // Hıza göre basılma oranını dinamik yapabiliriz, max = squashAmount
        float actualSquash = Mathf.Lerp(0.8f, squashAmount, Mathf.InverseLerp(2f, 15f, impactSpeed));
        
        // X genişler, Y basılır
        Vector3 squashedScale = new Vector3(_originalScale.x * (1f + (1f - actualSquash)), _originalScale.y * actualSquash, _originalScale.z);

        // Hızlıca basıl
        float elapsed = 0f;
        float durationIn = 0.05f; // Çok hızlı
        Vector3 startScale = transform.localScale;

        while (elapsed < durationIn)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, squashedScale, elapsed / durationIn);
            yield return null;
        }

        // Yavaşça toparlan
        elapsed = 0f;
        float durationOut = 1f / squashRecoverySpeed;

        while (elapsed < durationOut)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(squashedScale, _originalScale, elapsed / durationOut);
            yield return null;
        }

        transform.localScale = _originalScale;
        _isSquashing = false;
    }
}
