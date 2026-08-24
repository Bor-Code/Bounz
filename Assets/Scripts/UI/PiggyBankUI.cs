using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PiggyBankUI : MonoBehaviour
{
    [SerializeField] private TMP_Text capacityText;
    [SerializeField] private Slider fillSlider;
    [SerializeField] private Button smashButton;
    [SerializeField] private GameObject fullBadge;

    private void Start()
    {
        if (smashButton != null)
        {
            smashButton.onClick.AddListener(OnSmashClicked);
        }
        UpdateUI();
    }

    private void OnEnable()
    {
        if (PiggyBankManager.Instance != null)
        {
            PiggyBankManager.Instance.OnPiggyBankUpdated += UpdateUI;
            PiggyBankManager.Instance.OnPiggyBankSmashed += HandleSmashed;
        }
    }

    private void OnDisable()
    {
        if (PiggyBankManager.Instance != null)
        {
            PiggyBankManager.Instance.OnPiggyBankUpdated -= UpdateUI;
            PiggyBankManager.Instance.OnPiggyBankSmashed -= HandleSmashed;
        }
    }

    private void UpdateUI()
    {
        if (PiggyBankManager.Instance == null) return;

        int current = PiggyBankManager.Instance.CurrentCoins;
        int max = PiggyBankManager.Instance.CurrentCapacity;

        if (capacityText != null)
        {
            capacityText.text = $"{current} / {max}";
        }

        if (fillSlider != null)
        {
            fillSlider.value = max > 0 ? (float)current / max : 0f;
        }

        bool hasCoins = current > 0;
        bool isFull = PiggyBankManager.Instance.IsFull();
        if (smashButton != null) smashButton.interactable = hasCoins;
        if (fullBadge != null) fullBadge.SetActive(isFull);
    }

    private void OnSmashClicked()
    {
        PiggyBankManager.Instance?.PurchaseAndSmashPiggyBank();
    }

    private void HandleSmashed()
    {
        UpdateUI();
    }
}
