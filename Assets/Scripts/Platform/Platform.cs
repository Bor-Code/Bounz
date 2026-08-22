using UnityEngine;

/// <summary>
/// Tek bir platform prefab'ı üzerinde tüm davranışları barındırır.
/// Object pool ile uyumlu çalışmak için AddComponent yerine
/// mevcut bileşenleri etkinleştirme/devre dışı bırakma yaklaşımı kullanılır.
///
/// Prefab kurulumu:
///   Platform (bu script + SpriteRenderer + BoxCollider2D)
///   ├── [Component] SpringPlatform    — başlangıçta disabled
///   ├── [Component] FragilePlatform   — başlangıçta disabled
///   └── [Component] SizeShiftPlatform — başlangıçta disabled
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Platform : MonoBehaviour
{
    public static readonly Color ColorSafe      = new Color(0.29f, 0.86f, 0.44f);
    public static readonly Color ColorSpring    = new Color(0.98f, 0.87f, 0.26f);
    public static readonly Color ColorFragile   = new Color(0.95f, 0.29f, 0.29f);
    public static readonly Color ColorSizeShift = new Color(0.62f, 0.27f, 0.93f);

    private SpriteRenderer    _sr;
    private BoxCollider2D     _col;
    private SpringPlatform    _spring;
    private FragilePlatform   _fragile;
    private SizeShiftPlatform _sizeShift;

    public PlatformType Type { get; private set; }

    private void Awake()
    {
        _sr        = GetComponent<SpriteRenderer>();
        _col       = GetComponent<BoxCollider2D>();
        _spring    = GetComponent<SpringPlatform>();
        _fragile   = GetComponent<FragilePlatform>();
        _sizeShift = GetComponent<SizeShiftPlatform>();

        // Prefab'da hepsi disabled gelir
        SetBehaviours(false, false, false);
    }

    /// <summary>
    /// Havuzdan alınınca çağrılır; tipi ve genişliği ayarlar.
    /// </summary>
    public void Initialize(PlatformType type, float width)
    {
        Type = type;

        // Collider boyutu
        _col.size = new Vector2(width, _col.size.y);

        // Scale'i sıfırla (önceki kullanımdan kalmış olabilir)
        transform.localScale = Vector3.one;

        // Renk
        _sr.color = type switch
        {
            PlatformType.Spring    => ColorSpring,
            PlatformType.Fragile   => ColorFragile,
            PlatformType.SizeShift => ColorSizeShift,
            _                      => ColorSafe
        };

        // Davranış bileşenlerini ayarla
        SetBehaviours(
            type == PlatformType.Spring,
            type == PlatformType.Fragile,
            type == PlatformType.SizeShift);

        // Her bileşeni sıfırla
        _spring?.ResetState();
        _fragile?.ResetState();
        _sizeShift?.ResetState();
    }

    /// <summary>
    /// Havuza geri döndürülmeden önce temizlik.
    /// PlatformSpawner tarafından çağrılır.
    /// </summary>
    public void Cleanup()
    {
        SetBehaviours(false, false, false);
    }

    private void SetBehaviours(bool spring, bool fragile, bool sizeShift)
    {
        if (_spring    != null) _spring.enabled    = spring;
        if (_fragile   != null) _fragile.enabled   = fragile;
        if (_sizeShift != null) _sizeShift.enabled = sizeShift;
    }
}
