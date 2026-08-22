using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BattlePassUI : MonoBehaviour
{
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private Button buyPremiumButton;
    [SerializeField] private GameObject premiumBadge;

    private void Start()
    {
        if (buyPremiumButton != null)
        {
            buyPremiumButton.onClick.AddListener(OnBuyPremiumClicked);
        }
        UpdateUI();
    }

    private void OnEnable()
    {
        if (BattlePassManager.Instance != null)
        {
            BattlePassManager.Instance.OnPassUpdated += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (BattlePassManager.Instance != null)
        {
            BattlePassManager.Instance.OnPassUpdated -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (BattlePassManager.Instance == null) return;

        if (tokensText != null)
        {
            tokensText.text = $"Tokens: {BattlePassManager.Instance.CurrentTokens}";
        }

        bool isPremium = BattlePassManager.Instance.IsPremiumOwned;
        if (buyPremiumButton != null)
        {
            buyPremiumButton.gameObject.SetActive(!isPremium);
        }
        if (premiumBadge != null)
        {
            premiumBadge.SetActive(isPremium);
        }
    }

    private void OnBuyPremiumClicked()
    {
        if (BattlePassManager.Instance != null)
        {
            BattlePassManager.Instance.BuyPremiumPass();
        }
    }

    public void OnClaimFreeClicked(int tierIndex)
    {
        if (BattlePassManager.Instance != null)
        {
            BattlePassManager.Instance.ClaimFreeReward(tierIndex);
        }
    }

    public void OnClaimPremiumClicked(int tierIndex)
    {
        if (BattlePassManager.Instance != null)
        {
            BattlePassManager.Instance.ClaimPremiumReward(tierIndex);
        }
    }
}
