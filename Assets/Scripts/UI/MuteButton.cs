using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD'a eklenen küçük ses açma/kapama butonu.
/// AudioManager.Instance üzerinden müzik ve SFX volume'unu kontrol eder.
/// PlayerPrefs'e kaydeder, sahne yeniden yüklenince hatırlar.
///
/// Canvas > HUD altına boş bir GameObject ekle → MuteButton bileşenini ata.
/// </summary>
public class MuteButton : MonoBehaviour
{
    private const string MuteKey = "IsMuted";

    [Header("UI")]
    [SerializeField] private Button button;
    [Tooltip("Ses açıkken gösterilecek ikon/metin.")]
    [SerializeField] private TMP_Text label;

    [Tooltip("Ses açık sembolü.")]
    [SerializeField] private string iconOn  = "🔊";

    [Tooltip("Ses kapalı sembolü.")]
    [SerializeField] private string iconOff = "🔇";

    private bool _isMuted;

    private void Start()
    {
        _isMuted = PlayerPrefs.GetInt(MuteKey, 0) == 1;
        ApplyMute(_isMuted);

        if (button != null)
            button.onClick.AddListener(ToggleMute);

        UpdateLabel();
    }

    private void ToggleMute()
    {
        _isMuted = !_isMuted;
        PlayerPrefs.SetInt(MuteKey, _isMuted ? 1 : 0);
        PlayerPrefs.Save();
        ApplyMute(_isMuted);
        UpdateLabel();
    }

    private void ApplyMute(bool mute)
    {
        AudioListener.volume = mute ? 0f : 1f;

        // Haptics da kapat
        if (HapticManager.Instance != null)
            HapticManager.Instance.HapticsEnabled = !mute;
    }

    private void UpdateLabel()
    {
        if (label != null)
            label.text = _isMuted ? iconOff : iconOn;
    }
}
