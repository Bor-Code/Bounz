using UnityEngine;

/// <summary>
/// Kamera hareketi ile sonsuz paralaks arka plan oluşturur.
/// Her katman (layer) farklı bir hızda kayar; derinlik hissi yaratır.
///
/// Kullanım:
///   1. Her arka plan katmanı için ayrı bir Sprite Renderer objesi oluştur.
///   2. Bu scripti her birine ayrı ayrı ekle veya tek bir yönetici GameObject'e ekle.
///   3. Inspector'da Camera'yı ve paralaks çarpanını ayarla.
///
/// "Infinite tiling" için SpriteRenderer'ın Draw Mode'unu Tiled yapabilirsin
/// ya da iki kopya yan yana koyabilirsin — bu script pozisyonu yönetir.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Referans")]
    [SerializeField] private Camera cam;

    [Header("Katman Ayarları")]
    [Tooltip("0 = kamerayla aynı hızda (hareket etmez), 1 = sabit (tam paralaks).")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactorX = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactorY = 0.1f;

    [Tooltip("Arka planın tiling genişliği (sprite genişliği). Sonsuz kaydırma için kullanılır.")]
    [SerializeField] private float tileWidth = 20f;

    private Vector3 _lastCamPos;
    private float   _startX;

    private void Start()
    {
        if (cam == null) cam = Camera.main;
        _lastCamPos = cam.transform.position;
        _startX     = transform.position.x;
    }

    private void LateUpdate()
    {
        if (cam == null) return;

        Vector3 delta = cam.transform.position - _lastCamPos;

        // Paralaks kayma
        float moveX = delta.x * (1f - parallaxFactorX);
        float moveY = delta.y * (1f - parallaxFactorY);
        transform.position += new Vector3(moveX, moveY, 0f);

        _lastCamPos = cam.transform.position;

        // Sonsuz tiling — kameradan çok uzaklaşınca yana zıpla
        float distFromCam = cam.transform.position.x - transform.position.x;
        if (Mathf.Abs(distFromCam) >= tileWidth)
        {
            float sign = Mathf.Sign(distFromCam);
            transform.position += new Vector3(sign * tileWidth, 0f, 0f);
        }
    }
}
