using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerConfig config;

    [Header("References")]
    [SerializeField] private GroundDetector groundDetector;

    private Rigidbody2D _rb;
    private InputHandler _input;

    private float _chargeTimer = 0f;
    private bool _isCharging = false;
    private bool _jumpRequested = false;
    private float _pendingJumpForce = 0f;
    private float _pendingChargeRatio = 0f;
    private bool _wasGrounded = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _input = GetComponent<InputHandler>();
        _rb.gravityScale = config.gravityMultiplier;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable()
    {
        _input.onPressStarted += OnPressStarted;
        _input.onPressEnded   += OnPressEnded;
    }

    private void OnDisable()
    {
        _input.onPressStarted -= OnPressStarted;
        _input.onPressEnded   -= OnPressEnded;
    }

    private void Update()
    {
        HandleCharge();
        HandleHorizontalMovement();
    }

    private void FixedUpdate()
    {
        // Landing tespiti
        bool isGrounded = groundDetector.IsGrounded;
        if (isGrounded && !_wasGrounded)
        {
            float impactSpeed = Mathf.Abs(_rb.linearVelocity.y);
            GameEvents.RaisePlayerLanded(impactSpeed);
        }
        _wasGrounded = isGrounded;

        if (_jumpRequested && isGrounded)
        {
            GameEvents.RaisePlayerJumped(_pendingChargeRatio);
            ExecuteJump(_pendingJumpForce);
            _jumpRequested = false;
            _pendingJumpForce = 0f;
            _pendingChargeRatio = 0f;
        }
    }

    private void OnPressStarted()
    {
        if (GameManager.Instance?.State != GameManager.GameState.Playing) return;
        _chargeTimer = 0f;
        _isCharging = true;
    }

    private void OnPressEnded()
    {
        if (GameManager.Instance?.State != GameManager.GameState.Playing) return;
        _isCharging = false;
        float chargeRatio = Mathf.Clamp01(_chargeTimer / config.chargeTime);
        _pendingJumpForce  = Mathf.Lerp(config.minJumpForce, config.maxJumpForce, chargeRatio);
        _pendingChargeRatio = chargeRatio;
        _jumpRequested = true;
        _chargeTimer = 0f;
    }

    private void HandleCharge()
    {
        if (_isCharging)
            _chargeTimer = Mathf.Min(_chargeTimer + Time.deltaTime, config.chargeTime);
    }

    private void HandleHorizontalMovement()
    {
        if (GameManager.Instance?.State != GameManager.GameState.Playing) return;
        Vector2 vel = _rb.linearVelocity;
        vel.x = config.moveSpeed;
        _rb.linearVelocity = vel;
    }

    private void ExecuteJump(float force)
    {
        Vector2 vel = _rb.linearVelocity;
        vel.y = 0f;
        _rb.linearVelocity = vel;
        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    public float ChargeRatio => Mathf.Clamp01(_chargeTimer / config.chargeTime);
    public bool IsCharging => _isCharging;

    public void ApplyScaleChange(float multiplier)
    {
        float newScale = Mathf.Clamp(transform.localScale.x * multiplier, config.minScale, config.maxScale);
        transform.localScale = Vector3.one * newScale;
    }

    public void ForceJump(float force)
    {
        ExecuteJump(force);
    }
}
