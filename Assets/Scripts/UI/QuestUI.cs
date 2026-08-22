using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private Transform questContainer;
    [SerializeField] private GameObject questItemPrefab;
    [SerializeField] private Button closeButton;
    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }
    private void Start()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
    }
    public void OpenPanel()
    {
        if (questPanel != null)
            questPanel.SetActive(true);
        RefreshUI();
    }
    public void ClosePanel()
    {
        if (questPanel != null)
            questPanel.SetActive(false);
    }
    private void RefreshUI()
    {
        if (QuestManager.Instance == null) return;
        foreach (Transform child in questContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var quest in QuestManager.Instance.activeQuests)
        {
            GameObject item = Instantiate(questItemPrefab, questContainer);
            QuestItemUI itemUI = item.GetComponent<QuestItemUI>();
            if (itemUI != null)
            {
                itemUI.Setup(quest, () => OnClaimClicked(quest.id));
            }
        }
    }
    private void OnClaimClicked(string questId)
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ClaimReward(questId);
            RefreshUI(); 
        }
    }
}