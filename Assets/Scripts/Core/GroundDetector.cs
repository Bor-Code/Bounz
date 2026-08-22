using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    private bool _isGrounded = false;
    private Collider2D _currentPlatform = null;

    public bool IsGrounded => _isGrounded;
    public Collider2D CurrentPlatform => _currentPlatform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            _isGrounded = true;
            _currentPlatform = other;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            _isGrounded = false;
            _currentPlatform = null;
        }
    }
}
