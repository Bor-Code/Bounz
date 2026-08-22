using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class QuestItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private Button claimButton;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private GameObject completedBadge;
    public void Setup(QuestManager.Quest quest, Action onClaimClicked)
    {
        if (titleText != null)
        {
            titleText.text = GetTitleForQuestType(quest.type, quest.targetAmount);
        }
        if (progressText != null)
        {
            progressText.text = $"{quest.currentProgress} / {quest.targetAmount}";
        }
        if (progressBarFill != null)
        {
            progressBarFill.fillAmount = (float)quest.currentProgress / quest.targetAmount;
        }
        if (rewardText != null)
        {
            rewardText.text = $"+{quest.reward} Coin";
        }
        bool canClaim = quest.isCompleted && !quest.isRewardClaimed;
        if (claimButton != null)
        {
            claimButton.gameObject.SetActive(!quest.isRewardClaimed);
            claimButton.interactable = canClaim;
            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(() => onClaimClicked?.Invoke());
        }
        if (completedBadge != null)
        {
            completedBadge.SetActive(quest.isRewardClaimed);
        }
    }
    private string GetTitleForQuestType(QuestManager.QuestType type, int target)
    {
        return type switch
        {
            QuestManager.QuestType.CollectCoins => $"{target} Altın Topla",
            QuestManager.QuestType.PerfectLandings => $"{target} Kere Mükemmel İn",
            QuestManager.QuestType.PlayGames => $"{target} Oyun Oyna",
            _ => "Bilinmeyen Görev"
        };
    }
}