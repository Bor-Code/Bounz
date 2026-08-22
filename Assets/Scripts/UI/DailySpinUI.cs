using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DailySpinUI : MonoBehaviour
{
    [SerializeField] private Button spinButton;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Transform wheelTransform;

    private bool _isSpinning = false;

    private void Start()
    {
        if (spinButton != null)
        {
            spinButton.onClick.AddListener(OnSpinClicked);
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (DailySpinManager.Instance != null && spinButton != null)
        {
            spinButton.interactable = DailySpinManager.Instance.CanSpin();
        }
    }

    private void OnEnable()
    {
        if (DailySpinManager.Instance != null)
        {
            DailySpinManager.Instance.OnRewardSpun += HandleRewardSpun;
        }
    }

    private void OnDisable()
    {
        if (DailySpinManager.Instance != null)
        {
            DailySpinManager.Instance.OnRewardSpun -= HandleRewardSpun;
        }
    }

    private void OnSpinClicked()
    {
        if (_isSpinning) return;
        if (DailySpinManager.Instance != null && DailySpinManager.Instance.CanSpin())
        {
            _isSpinning = true;
            spinButton.interactable = false;
            
            if (wheelTransform != null)
            {
                StartCoroutine(SpinAnimationRoutine());
            }
            else
            {
                DailySpinManager.Instance.SpinWheel();
            }
        }
    }

    private IEnumerator SpinAnimationRoutine()
    {
        float duration = 3f;
        float elapsed = 0f;
        float startZ = wheelTransform.eulerAngles.z;
        float targetZ = startZ - (360f * 5f) - UnityEngine.Random.Range(0, 360f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            
            float currentZ = Mathf.Lerp(startZ, targetZ, easeT);
            wheelTransform.rotation = Quaternion.Euler(0, 0, currentZ);
            
            yield return null;
        }

        if (DailySpinManager.Instance != null)
        {
            DailySpinManager.Instance.SpinWheel();
        }
    }

    private void HandleRewardSpun(SpinReward reward)
    {
        _isSpinning = false;
        if (rewardPanel != null) rewardPanel.SetActive(true);
        if (rewardText != null) rewardText.text = $"TEBRİKLER!\n{reward.rewardName}\n+{reward.coinAmount} Coin";
        UpdateUI();
    }

    public void CloseRewardPanel()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }
}
