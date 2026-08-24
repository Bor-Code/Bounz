

using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System;

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

    public event Action OnStoreClosed;

    private void Awake()

    {

        if (closeButton != null)

            closeButton.onClick.AddListener(CloseStore);

    }

    private void Start()

    {

        if (storePanel != null) storePanel.SetActive(false);

    }

    public void OpenStore()

    {

        if (storePanel != null) storePanel.SetActive(true);

        RefreshUI();

    }

    public void CloseStore()

    {

        if (storePanel != null) storePanel.SetActive(false);

        OnStoreClosed?.Invoke();

    }

    private void RefreshUI()

    {

        if (SkinManager.Instance == null) return;

        if (skinConfig == null) skinConfig = SkinConfig.CreateDefault();

        if (totalScoreText != null)

        {

            totalScoreText.text = $"Wallet: {SkinManager.Instance.GetTotalScore()}";

        }

        if (contentContainer == null || skinItemPrefab == null) return;

        foreach (Transform child in contentContainer)

        {

            Destroy(child.gameObject);

        }

        string activeSkinId = SkinManager.Instance.GetSelectedSkinId();

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

        RefreshUI();

    }

}

