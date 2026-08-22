using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Market (Skin Store) ekranını yönetir.
/// StartScreen'den açılır. Mevcut skinleri listeler, satın alma ve donanma işlemlerini yapar.
/// </summary>
public class SkinStoreUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinConfig skinConfig;
    [SerializeField] private GameObject storePanel;
    
    [Header("UI Elements")]
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private Transform contentContainer;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseStore);
    }

    private void Start()
    {
        storePanel.SetActive(false);
    }

    public void OpenStore()
    {
        storePanel.SetActive(true);
        RefreshUI();
    }

    public void CloseStore()
    {
        storePanel.SetActive(false);
    }

    private void RefreshUI()
    {
        if (SkinManager.Instance == null || skinConfig == null) return;

        // Total score güncelle
        if (totalScoreText != null)
        {
            totalScoreText.text = $"Wallet: {SkinManager.Instance.GetTotalScore()}";
        }

        // Mevcut listeyi temizle
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        string activeSkinId = SkinManager.Instance.GetSelectedSkinId();

        // Butonları oluştur
        foreach (SkinData skin in skinConfig.skins)
        {
            GameObject item = Instantiate(skinItemPrefab, contentContainer);
            SkinItemUI itemUI = item.GetComponent<SkinItemUI>();
            if (itemUI != null)
            {
                bool isUnlocked = SkinManager.Instance.IsSkinUnlocked(skin.id);
                bool isSelected = skin.id == activeSkinId;

                itemUI.Setup(skin, isUnlocked, isSelected, OnSkinButtonClicked);
            }
        }
    }

    private void OnSkinButtonClicked(SkinData skin, bool isUnlocked)
    {
        if (isUnlocked)
        {
            SkinManager.Instance.SelectSkin(skin.id);
        }
        else
        {
            SkinManager.Instance.BuySkin(skin.id, skin.price);
        }
        RefreshUI(); // Seçim sonrası ekranı yenile
    }
}
