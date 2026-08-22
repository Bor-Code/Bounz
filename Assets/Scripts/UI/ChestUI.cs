using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChestUI : MonoBehaviour
{
    [SerializeField] private Button openChestButton;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text rewardText;

    private void Start()
    {
        if (openChestButton != null)
        {
            openChestButton.onClick.AddListener(OnOpenChestClicked);
        }
    }

    private void OnEnable()
    {
        if (ChestManager.Instance != null)
        {
            ChestManager.Instance.OnSkinWon += HandleSkinWon;
            ChestManager.Instance.OnCoinsWon += HandleCoinsWon;
            ChestManager.Instance.OnChestFailed += HandleChestFailed;
        }
    }

    private void OnDisable()
    {
        if (ChestManager.Instance != null)
        {
            ChestManager.Instance.OnSkinWon -= HandleSkinWon;
            ChestManager.Instance.OnCoinsWon -= HandleCoinsWon;
            ChestManager.Instance.OnChestFailed -= HandleChestFailed;
        }
    }

    private void OnOpenChestClicked()
    {
        if (openChestButton != null) openChestButton.interactable = false;
        if (ChestManager.Instance != null) ChestManager.Instance.OpenChest();
    }

    private void HandleSkinWon(string skinId)
    {
        ShowReward($"YENİ KOSTÜM!\n{skinId}");
    }

    private void HandleCoinsWon(int amount)
    {
        ShowReward($"BÜYÜK İKRAMİYE!\n+{amount} COIN");
    }

    private void HandleChestFailed()
    {
        if (openChestButton != null) openChestButton.interactable = true;
    }

    private void ShowReward(string message)
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);
        if (rewardText != null) rewardText.text = message;
        if (openChestButton != null) openChestButton.interactable = true;
    }
}
