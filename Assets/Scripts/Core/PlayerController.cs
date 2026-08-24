using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputHandler))]
public class PlayerController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerConfig config;

    [Header("References")]
    [SerializeField] private GroundDetector groundDetector;

    [Header("Power Ups")]
    [SerializeField] private float defaultShieldDuration = 5f;

    private Rigidbody2D _rb;
    private InputHandler _input;
    private float _chargeTimer = 0f;
    private bool _isCharging = false;
    private bool _jumpRequested = false;
    private float _pendingJumpForce = 0f;
    private float _pendingChargeRatio = 0f;
    private bool _wasGrounded = false;
    private float _shieldTimer = 0f;
    private float _magnetTimer = 0f;
    private float _multiplierTimer = 0f;
    private int _comboCount = 0;

    private const float PerfectLandingThreshold = 0.35f;

    public float ChargeRatio => Mathf.Clamp01(_chargeTimer / Mathf.Max(0.01f, config.chargeTime));
    public bool IsCharging => _isCharging;
    public bool IsShielded { get; private set; }
    public bool IsMagnetActive { get; private set; }
    public bool IsMultiplierActive { get; private set; }

    private void Awake()
    {
        EnsureReferences();
        _rb.gravityScale = config.gravityMultiplier;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnEnable()
    {
        EnsureReferences();
        _input.onPressStarted += OnPressStarted;
        _input.onPressEnded += OnPressEnded;
    }

    private void OnDisable()
    {
        if (_input == null) return;

        _input.onPressStarted -= OnPressStarted;
        _input.onPressEnded -= OnPressEnded;
    }

    private void Update()
    {
        HandleCharge();
        HandleHorizontalMovement();
        HandleShield();
        HandleMagnet();
        HandleMultiplier();
    }

    private void FixedUpdate()
    {
        if (groundDetector == null) return;

        bool isGrounded = groundDetector.IsGrounded;
        if (isGrounded && !_wasGrounded)
        {
            float impactSpeed = Mathf.Abs(_rb.linearVelocity.y);
            GameEvents.RaisePlayerLanded(impactSpeed);

            if (groundDetector.CurrentPlatform != null)
            {
                float platformX = groundDetector.CurrentPlatform.transform.position.x;
                float playerX = transform.position.x;
                if (Mathf.Abs(playerX - platformX) <= PerfectLandingThreshold)
                {
                    _comboCount++;
                    GameEvents.RaisePerfectLanding(_comboCount, transform.position);
                    ScoreManager.Instance?.AddComboScore(_comboCount * 10);
                }
                else if (_comboCount > 0)
                {
                    _comboCount = 0;
                    GameEvents.RaiseComboBroken();
                }
            }
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
        float chargeRatio = Mathf.Clamp01(_chargeTimer / Mathf.Max(0.01f, config.chargeTime));
        _pendingJumpForce = Mathf.Lerp(config.minJumpForce, config.maxJumpForce, chargeRatio);
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

    private void HandleShield()
    {
        if (!IsShielded) return;

        _shieldTimer -= Time.deltaTime;
        if (_shieldTimer <= 0f)
        {
            IsShielded = false;
            _shieldTimer = 0f;
        }
    }

    private void HandleMagnet()
    {
        if (!IsMagnetActive) return;

        _magnetTimer -= Time.deltaTime;
        if (_magnetTimer <= 0f)
        {
            IsMagnetActive = false;
            _magnetTimer = 0f;
        }
    }

    private void HandleMultiplier()
    {
        if (!IsMultiplierActive) return;

        _multiplierTimer -= Time.deltaTime;
        if (_multiplierTimer <= 0f)
        {
            IsMultiplierActive = false;
            _multiplierTimer = 0f;
            if (ScoreManager.Instance != null) ScoreManager.Instance.Multiplier = 1f;
        }
    }

    private void ExecuteJump(float force)
    {
        Vector2 vel = _rb.linearVelocity;
        vel.y = 0f;
        _rb.linearVelocity = vel;
        _rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    public void ActivateShield()
    {
        float duration = UpgradeManager.Instance != null
            ? UpgradeManager.Instance.GetUpgradeValue(UpgradeManager.ShieldDurationUpgradeId, defaultShieldDuration)
            : defaultShieldDuration;

        ActivateShield(duration);
    }

    public void ActivateShield(float duration)
    {
        IsShielded = true;
        _shieldTimer = Mathf.Max(0.1f, duration);
    }

    public void ConsumeShield()
    {
        IsShielded = false;
        _shieldTimer = 0f;
    }

    public void ActivateMagnet(float duration)
    {
        IsMagnetActive = true;
        _magnetTimer = duration;
    }

    public void ActivateMultiplier(float duration, float multiplierAmount)
    {
        IsMultiplierActive = true;
        _multiplierTimer = duration;
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.Multiplier = multiplierAmount;
        }
    }

    public void ApplyScaleChange(float multiplier)
    {
        float newScale = Mathf.Clamp(transform.localScale.x * multiplier, config.minScale, config.maxScale);
        transform.localScale = Vector3.one * newScale;
    }

    public void ForceJump(float force)
    {
        ExecuteJump(force);
    }

    private void EnsureReferences()
    {
        if (config == null) config = PlayerConfig.CreateDefault();
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        if (_input == null) _input = GetComponent<InputHandler>();
        if (groundDetector == null) groundDetector = GetComponentInChildren<GroundDetector>();

        if (groundDetector == null)
        {
            GameObject groundCheck = new GameObject("GroundDetector");
            groundCheck.transform.SetParent(transform);
            groundCheck.transform.localPosition = new Vector3(0f, -0.55f, 0f);
            CircleCollider2D col = groundCheck.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;
            groundDetector = groundCheck.AddComponent<GroundDetector>();
        }
    }
}
