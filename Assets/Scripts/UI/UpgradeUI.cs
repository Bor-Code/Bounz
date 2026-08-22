using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private GameObject upgradeItemPrefab;

    private void Start()
    {
        PopulateUpgrades();
        UpdateCoinUI();
    }

    private void OnEnable()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradesChanged += HandleUpgradesChanged;
        }
        if (SkinManager.Instance != null)
        {
            GameEvents.OnCoinCollected += HandleCoinCollected;
        }
    }

    private void OnDisable()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradesChanged -= HandleUpgradesChanged;
        }
        if (SkinManager.Instance != null)
        {
            GameEvents.OnCoinCollected -= HandleCoinCollected;
        }
    }

    private void HandleUpgradesChanged()
    {
        UpdateCoinUI();
        PopulateUpgrades(); // Sahnede gerçek bir Item scripti olsaydı sadece onu güncellerdik.
    }

    private void HandleCoinCollected(Vector3 _) => UpdateCoinUI();

    private void UpdateCoinUI()
    {
        if (coinText != null && SaveManager.Instance != null)
        {
            coinText.text = SaveManager.Instance.CurrentSave.totalScore.ToString();
        }
    }

    private void PopulateUpgrades()
    {
        if (UpgradeManager.Instance == null || contentPanel == null || upgradeItemPrefab == null) return;

        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        List<UpgradeStat> stats = UpgradeManager.Instance.GetAllUpgrades();
        
        foreach (var stat in stats)
        {
            GameObject go = Instantiate(upgradeItemPrefab, contentPanel);
            
            TMP_Text nameText = go.transform.Find("NameText")?.GetComponent<TMP_Text>();
            TMP_Text levelText = go.transform.Find("LevelText")?.GetComponent<TMP_Text>();
            TMP_Text costText = go.transform.Find("CostText")?.GetComponent<TMP_Text>();
            Button buyBtn = go.transform.Find("BuyButton")?.GetComponent<Button>();

            if (nameText != null) nameText.text = stat.displayName;
            if (levelText != null) levelText.text = $"Lv. {stat.currentLevel}/{stat.maxLevel}";
            
            if (costText != null)
            {
                if (stat.currentLevel >= stat.maxLevel)
                    costText.text = "MAX";
                else
                    costText.text = stat.GetNextCost().ToString();
            }

            if (buyBtn != null)
            {
                buyBtn.interactable = stat.currentLevel < stat.maxLevel;
                buyBtn.onClick.AddListener(() => 
                {
                    UpgradeManager.Instance.BuyUpgrade(stat.id);
                });
            }
        }
    }
}
