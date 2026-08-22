using UnityEngine;
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
    public void ResetState()
    {
        _pingPongTimer = 0f;
        _playerTransform = null;
        _isInitialized = false;
    }
    private void Update()
    {
        if (!_isInitialized)
        {
            _startPos = transform.position;
            _previousPos = transform.position;
            _isInitialized = true;
            _pingPongTimer = Random.Range(0f, 10f); 
            return;
        }
        _pingPongTimer += Time.deltaTime * speed;
        float pingPongValue = Mathf.PingPong(_pingPongTimer, moveDistance * 2f) - moveDistance;
        Vector3 newPos = _startPos + (moveAxis.normalized * pingPongValue);
        Vector3 delta = newPos - _previousPos;
        transform.position = newPos;
        if (_playerTransform != null)
        {
            _playerTransform.position += delta;
        }
        _previousPos = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") || col.gameObject.GetComponent<PlayerController>() != null)
        {
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