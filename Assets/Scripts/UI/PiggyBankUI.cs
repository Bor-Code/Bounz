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
            fillSlider.value = (float)current / max;
        }

        bool isFull = PiggyBankManager.Instance.IsFull();
        if (fullBadge != null) fullBadge.SetActive(isFull);
        
        // IAP Satın alım butonu Kumbara dolu olmadan da tıklanabilir, veya sadece dolunca tıklanabilir yapılabilir.
        // Genelde dolu olduğunda en iyi değeri verir.
    }

    private void OnSmashClicked()
    {
        if (PiggyBankManager.Instance != null && PiggyBankManager.Instance.CurrentCoins > 0)
        {
            // Gerçekte burada IAP ödeme ekranı açılır
            PiggyBankManager.Instance.SmashPiggyBank();
        }
    }

    private void HandleSmashed()
    {
        // Kırılma animasyonları, coin patlama efektleri burada tetiklenebilir
        UpdateUI();
    }
}
