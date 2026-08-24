

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

        if (IsGround(other))

        {

            _isGrounded = true;

            _currentPlatform = other;

        }

    }

    private void OnTriggerExit2D(Collider2D other)

    {

        if (IsGround(other))

        {

            _isGrounded = false;

            _currentPlatform = null;

        }

    }

    private bool IsGround(Collider2D other)

    {

        if (groundLayer.value == 0)

            return other.GetComponent<Platform>() != null;

        return ((1 << other.gameObject.layer) & groundLayer) != 0;

    }

}

