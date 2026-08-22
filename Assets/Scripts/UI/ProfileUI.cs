using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text xpText;
    [SerializeField] private Slider xpSlider;
    [SerializeField] private GameObject levelUpEffect;

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.OnLevelUp += HandleLevelUp;
        }
    }

    private void OnDisable()
    {
        if (ProfileManager.Instance != null)
        {
            ProfileManager.Instance.OnLevelUp -= HandleLevelUp;
        }
    }

    private void UpdateUI()
    {
        if (ProfileManager.Instance == null) return;

        if (levelText != null)
        {
            levelText.text = $"Lv. {ProfileManager.Instance.CurrentLevel}";
        }

        int currentXP = ProfileManager.Instance.CurrentXP;
        int requiredXP = ProfileManager.Instance.GetXPForNextLevel();

        if (xpText != null)
        {
            xpText.text = $"{currentXP} / {requiredXP} XP";
        }

        if (xpSlider != null)
        {
            xpSlider.value = (float)currentXP / requiredXP;
        }
    }

    private void HandleLevelUp(int newLevel)
    {
        UpdateUI();
        if (levelUpEffect != null)
        {
            levelUpEffect.SetActive(false);
            levelUpEffect.SetActive(true);
        }
    }
}
