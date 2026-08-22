using UnityEngine;

/// <summary>
/// Main Camera'ya eklenir; oynadığı süre boyunca Player'ı smooth takip eder.
///
/// Yatayda: lerp ile takip (hız ayarlanabilir).
/// Dikeyde: yukarı çıkarken hızlı takip eder, aşağı düşerken geciktirerek
///          kameranın platformlara "baktığı" hissi verir.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [Tooltip("Kameranın Player'ın gerisinde/ilerisinde ve yukarısında durduğu mesafe.")]
    [SerializeField] private Vector2 offset = new Vector2(-3f, 2f);

    [Header("Smoothing")]
    [Tooltip("Yatay takip hızı. Yüksek = daha sıkı.")]
    [SerializeField] private float horizontalSpeed = 5f;

    [Tooltip("Dikey yukarı takip hızı.")]
    [SerializeField] private float verticalUpSpeed = 4f;

    [Tooltip("Dikey aşağı takip hızı (daha yavaş = daha dramatik).")]
    [SerializeField] private float verticalDownSpeed = 1.5f;

    [Header("Bounds")]
    [Tooltip("Kameranın ulaşabileceği minimum Y değeri (zemin sınırı).")]
    [SerializeField] private float minY = -10f;

    // Oyun durumu
    private bool _isFollowing = false;

    private void Awake()
    {
        // GameManager'dan bağımsız olarak da kullanılabilsin
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += OnGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= OnGameOver;
    }

    /// <summary>Takibi aktif eder (GameManager ya da MainMenu çağırır).</summary>
    public void StartFollowing(Transform followTarget)
    {
        target = followTarget;
        _isFollowing = true;

        // Kamerayı hemen player pozisyonuna ışınla (ilk kare "sıçraması" önle)
        Vector3 snap = new Vector3(
            followTarget.position.x + offset.x,
            Mathf.Max(followTarget.position.y + offset.y, minY),
            transform.position.z);
        transform.position = snap;
    }

    public void StopFollowing() => _isFollowing = false;

    private void LateUpdate()
    {
        if (!_isFollowing || target == null) return;

        Vector3 current = transform.position;
        float targetX = target.position.x + offset.x;
        float targetY = Mathf.Max(target.position.y + offset.y, minY);

        // Dikey: yukarı vs aşağı farklı hız
        float ySpeed = (targetY > current.y) ? verticalUpSpeed : verticalDownSpeed;

        float newX = Mathf.Lerp(current.x, targetX, horizontalSpeed * Time.deltaTime);
        float newY = Mathf.Lerp(current.y, targetY, ySpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, current.z);
    }

    private void OnGameOver(int _, bool __)
    {
        // Oyun bitince takibi durdur; kamera son konumda kalır
        _isFollowing = false;
    }
}
