using UnityEngine;

/// <summary>
/// Platformun yatay (vektörel olarak ayarlanabilir) hareket etmesini sağlar.
/// Oyuncu üzerine zıpladığında oyuncuyu da beraberinde taşır (sürtünme simülasyonu).
/// Pool uyumludur.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Saniyede kat edeceği mesafe")]
    [SerializeField] private float speed = 2f;
    [Tooltip("Merkezden ne kadar uzağa gideceği (ping-pong mesafesi)")]
    [SerializeField] private float moveDistance = 3f;
    [Tooltip("Hangi yönde hareket edeceği (genelde (1,0,0) yani yatay)")]
    [SerializeField] private Vector3 moveAxis = Vector3.right;

    private Vector3 _startPos;
    private float _pingPongTimer = 0f;
    private Vector3 _previousPos;
    private Transform _playerTransform;
    private bool _isInitialized = false;

    /// <summary>Pool'dan alınınca çağrılır — temiz başlangıç.</summary>
    public void ResetState()
    {
        _pingPongTimer = 0f;
        _playerTransform = null;
        _isInitialized = false;
        // _startPos değerini Update'in ilk karesinde alacağız çünkü
        // Pool'dan çekilip position ayarlandığı an Awake/OnEnable sonrası olabilir.
    }

    private void Update()
    {
        // Pool'dan alındıktan sonra gerçek pozisyonunu kaydetmesi için 1 frame bekletme mantığı
        if (!_isInitialized)
        {
            _startPos = transform.position;
            _previousPos = transform.position;
            _isInitialized = true;
            
            // Farklı platformların aynı anda aynı yöne gitmemesi için ufak bir rastgele offset
            _pingPongTimer = Random.Range(0f, 10f); 
            return;
        }

        _pingPongTimer += Time.deltaTime * speed;
        
        // Mathf.PingPong 0 ile moveDistance arasında gider gelir, biz -moveDistance ile +moveDistance arası istiyoruz
        float pingPongValue = Mathf.PingPong(_pingPongTimer, moveDistance * 2f) - moveDistance;
        
        Vector3 newPos = _startPos + (moveAxis.normalized * pingPongValue);
        
        // Hareket deltası
        Vector3 delta = newPos - _previousPos;
        transform.position = newPos;

        // Üzerinde duran oyuncuyu taşı
        if (_playerTransform != null)
        {
            _playerTransform.position += delta;
        }

        _previousPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Yukarıdan inen oyuncuyu parent yapmadan taşıma (fizik hatalarını önler)
        if (col.gameObject.CompareTag("Player") || col.gameObject.GetComponent<PlayerController>() != null)
        {
            // Sadece yukarıdan geliyorsa (oyuncu düşüyorsa) taşı
            if (col.relativeVelocity.y <= 0f)
            {
                _playerTransform = col.transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.transform == _playerTransform)
        {
            _playerTransform = null;
        }
    }
}
