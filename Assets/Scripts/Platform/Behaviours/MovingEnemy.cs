using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class MovingEnemy : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float moveDistance = 2f;
    private Vector3 _startPos;
    private int _direction = 1;
    private void Start()
    {
        _startPos = transform.localPosition;
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.State != GameManager.GameState.Playing)
            return;
        float limitRight = _startPos.x + moveDistance;
        float limitLeft  = _startPos.x - moveDistance;
        transform.Translate(Vector3.right * (_direction * speed * Time.deltaTime));
        if (transform.localPosition.x > limitRight)
        {
            transform.localPosition = new Vector3(limitRight, transform.localPosition.y, transform.localPosition.z);
            _direction = -1;
        }
        else if (transform.localPosition.x < limitLeft)
        {
            transform.localPosition = new Vector3(limitLeft, transform.localPosition.y, transform.localPosition.z);
            _direction = 1;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            if (player.IsShielded)
            {
                player.ConsumeShield();
                Destroy(gameObject); 
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
    }
}