using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour
{
    public static readonly Color ColorSafe      = new Color(0.29f, 0.86f, 0.44f);
    public static readonly Color ColorSpring    = new Color(0.98f, 0.87f, 0.26f);
    public static readonly Color ColorFragile   = new Color(0.95f, 0.29f, 0.29f);
    public static readonly Color ColorSizeShift = new Color(0.62f, 0.27f, 0.93f);
    public static readonly Color ColorMoving    = new Color(0.2f, 0.6f, 1f);

    private SpriteRenderer    _sr;
    private BoxCollider2D     _col;
    private SpringPlatform    _spring;
    private FragilePlatform   _fragile;
    private SizeShiftPlatform _sizeShift;
    private MovingPlatform    _moving;

    public PlatformType Type { get; private set; }

    private void Awake()
    {
        RefreshReferences();
        if (_sr.sprite == null) _sr.sprite = GameBootstrapper.CreateRuntimeSprite();
        SetBehaviours(false, false, false, false);
    }

    public void Initialize(PlatformType type, float width)
    {
        Type = type;
        RefreshReferences();

        _col.enabled = true;
        _sr.enabled = true;
        _col.size = new Vector2(width, _col.size.y <= 0f ? 0.4f : _col.size.y);
        transform.localScale = Vector3.one;

        _sr.color = type switch
        {
            PlatformType.Spring    => ColorSpring,
            PlatformType.Fragile   => ColorFragile,
            PlatformType.SizeShift => ColorSizeShift,
            PlatformType.Moving    => ColorMoving,
            _                      => ColorSafe
        };

        SetBehaviours(
            type == PlatformType.Spring,
            type == PlatformType.Fragile,
            type == PlatformType.SizeShift,
            type == PlatformType.Moving);

        _spring?.ResetState();
        _fragile?.ResetState();
        _sizeShift?.ResetState();
        _moving?.ResetState();
    }

    public void Cleanup()
    {
        RefreshReferences();
        SetBehaviours(false, false, false, false);

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (IsSpawnedRuntimeChild(child))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void MarkCollapsed()
    {
        if (_col != null) _col.enabled = false;
        if (_sr != null) _sr.enabled = false;
    }

    private void RefreshReferences()
    {
        _sr        = GetComponent<SpriteRenderer>();
        _col       = GetComponent<BoxCollider2D>();
        _spring    = GetComponent<SpringPlatform>();
        _fragile   = GetComponent<FragilePlatform>();
        _sizeShift = GetComponent<SizeShiftPlatform>();
        _moving    = GetComponent<MovingPlatform>();

        if (_sr != null && _sr.sprite == null)
        {
            _sr.sprite = GameBootstrapper.CreateRuntimeSprite();
        }
    }

    private bool IsSpawnedRuntimeChild(Transform child)
    {
        return child.GetComponent<Coin>() != null
            || child.GetComponent<Spike>() != null
            || child.GetComponent<MovingEnemy>() != null
            || child.GetComponent<ShieldPowerUp>() != null
            || child.GetComponent<MagnetPowerUp>() != null
            || child.GetComponent<ScoreMultiplierPowerUp>() != null;
    }

    private void SetBehaviours(bool spring, bool fragile, bool sizeShift, bool moving)
    {
        if (_spring    != null) _spring.enabled    = spring;
        if (_fragile   != null) _fragile.enabled   = fragile;
        if (_sizeShift != null) _sizeShift.enabled = sizeShift;
        if (_moving    != null) _moving.enabled    = moving;
    }
}
