using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour
{
    public static readonly Color ColorSafe      = new Color(0.29f, 0.86f, 0.44f);
    public static readonly Color ColorSpring    = new Color(0.98f, 0.87f, 0.26f);
    public static readonly Color ColorFragile   = new Color(0.95f, 0.29f, 0.29f);
    public static readonly Color ColorSizeShift = new Color(0.62f, 0.27f, 0.93f);

    private SpriteRenderer _sr;
    private BoxCollider2D _col;

    public PlatformType Type { get; private set; }

    private void Awake()
    {
        _sr  = GetComponent<SpriteRenderer>();
        _col = GetComponent<BoxCollider2D>();
    }

    public void Initialize(PlatformType type, float width)
    {
        Type = type;
        _col.size = new Vector2(width, _col.size.y);
        _sr.color = type switch
        {
            PlatformType.Spring    => ColorSpring,
            PlatformType.Fragile   => ColorFragile,
            PlatformType.SizeShift => ColorSizeShift,
            _                      => ColorSafe
        };
    }
}
