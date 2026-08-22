using UnityEngine;
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
        float moveX = delta.x * (1f - parallaxFactorX);
        float moveY = delta.y * (1f - parallaxFactorY);
        transform.position += new Vector3(moveX, moveY, 0f);
        _lastCamPos = cam.transform.position;
        float distFromCam = cam.transform.position.x - transform.position.x;
        if (Mathf.Abs(distFromCam) >= tileWidth)
        {
            float sign = Mathf.Sign(distFromCam);
            transform.position += new Vector3(sign * tileWidth, 0f, 0f);
        }
    }
}