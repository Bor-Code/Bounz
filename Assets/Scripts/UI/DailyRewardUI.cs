using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DailyRewardUI : MonoBehaviour
{
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private Button claimButton;
    [SerializeField] private TMP_Text rewardAmountText;
    [SerializeField] private TMP_Text streakText;
    private void Start()
    {
        if (DailyRewardManager.Instance != null && DailyRewardManager.Instance.CanClaimReward)
        {
            ShowPanel();
        }
        else
        {
            if (rewardPanel != null) rewardPanel.SetActive(false);
        }
        if (claimButton != null)
        {
            claimButton.onClick.AddListener(OnClaimClicked);
        }
    }
    public void ShowPanel()
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);
        if (DailyRewardManager.Instance != null)
        {
            if (rewardAmountText != null)
            {
                rewardAmountText.text = $"+{DailyRewardManager.Instance.GetNextRewardAmount()} Coin";
            }
            if (streakText != null)
            {
                streakText.text = $"Day {DailyRewardManager.Instance.CurrentStreak + 1}";
            }
        }
    }
    private void OnClaimClicked()
    {
        if (DailyRewardManager.Instance != null)
        {
            if (DailyRewardManager.Instance.ClaimReward())
            {
                if (rewardPanel != null) rewardPanel.SetActive(false);
                
                if (GameEvents.RaiseCoinCollected != null)
                {
                    // Efekt oynatmak için hayali bir pozisyonda para toplanmış gibi event tetiklenebilir.
                }
            }
        }
    }
}
