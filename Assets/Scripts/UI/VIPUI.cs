using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VIPUI : MonoBehaviour
{
    [SerializeField] private Button buyVIPButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject vipCrownIcon;

    private void Start()
    {
        if (buyVIPButton != null)
        {
            buyVIPButton.onClick.AddListener(OnBuyVIPClicked);
        }
        UpdateUI();
    }

    private void OnEnable()
    {
        if (VIPManager.Instance != null)
        {
            VIPManager.Instance.OnVIPStatusChanged += UpdateUI;
        }
    }

    private void OnDisable()
    {
        if (VIPManager.Instance != null)
        {
            VIPManager.Instance.OnVIPStatusChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (VIPManager.Instance == null) return;

        bool isVip = VIPManager.Instance.IsVIPActive;

        if (buyVIPButton != null) buyVIPButton.gameObject.SetActive(!isVip);
        if (vipCrownIcon != null) vipCrownIcon.SetActive(isVip);

        if (statusText != null)
        {
            statusText.text = isVip ? "VIP ÜYESİNİZ" : "VIP ÜYE OL (Aylık Abonelik)";
        }
    }

    private void OnBuyVIPClicked()
    {
        // Gerçekte IAPManager üzerinden satın alım tetiklenir
        if (VIPManager.Instance != null)
        {
            VIPManager.Instance.PurchaseVIP();
        }
    }
}
