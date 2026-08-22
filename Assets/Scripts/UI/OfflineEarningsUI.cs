using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OfflineEarningsUI : MonoBehaviour
{
    [SerializeField] private GameObject offlinePanel;
    [SerializeField] private TMP_Text earningsText;
    [SerializeField] private Button claimButton;

    private void Start()
    {
        if (OfflineEarningsManager.Instance != null && OfflineEarningsManager.Instance.PendingOfflineCoins > 0)
        {
            ShowPanel();
        }
        else
        {
            if (offlinePanel != null) offlinePanel.SetActive(false);
        }

        if (claimButton != null)
        {
            claimButton.onClick.AddListener(OnClaimClicked);
        }
    }

    private void ShowPanel()
    {
        if (offlinePanel != null) offlinePanel.SetActive(true);
        if (earningsText != null && OfflineEarningsManager.Instance != null)
        {
            earningsText.text = $"Sen yokken kumbaranda\n+{OfflineEarningsManager.Instance.PendingOfflineCoins} Coin birikti!";
        }
    }

    private void OnClaimClicked()
    {
        if (OfflineEarningsManager.Instance != null)
        {
            OfflineEarningsManager.Instance.ClaimOfflineEarnings();
        }
        
        if (offlinePanel != null) offlinePanel.SetActive(false);
    }
}
