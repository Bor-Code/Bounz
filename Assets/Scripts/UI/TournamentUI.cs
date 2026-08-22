using UnityEngine;
using TMPro;

public class TournamentUI : MonoBehaviour
{
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private TMP_Text timeRemainingText;
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text rewardMsgText;

    private void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        if (TournamentManager.Instance != null)
        {
            TournamentManager.Instance.OnLeaderboardUpdated += UpdateUI;
            TournamentManager.Instance.OnTournamentRewardClaimed += ShowRewardPanel;
        }
    }

    private void OnDisable()
    {
        if (TournamentManager.Instance != null)
        {
            TournamentManager.Instance.OnLeaderboardUpdated -= UpdateUI;
            TournamentManager.Instance.OnTournamentRewardClaimed -= ShowRewardPanel;
        }
    }

    private void Update()
    {
        if (TournamentManager.Instance != null && timeRemainingText != null)
        {
            System.TimeSpan remaining = TournamentManager.Instance.EndDate - System.DateTime.Now;
            if (remaining.TotalSeconds > 0)
            {
                timeRemainingText.text = $"{remaining.Days}g {remaining.Hours}s {remaining.Minutes}d";
            }
            else
            {
                timeRemainingText.text = "Sonuçlanıyor...";
            }
        }
    }

    private void UpdateUI()
    {
        if (TournamentManager.Instance == null || leaderboardText == null) return;

        string board = "HAFTALIK LİDERLİK TABLOSU\n\n";
        var list = TournamentManager.Instance.CurrentLeaderboard;

        for (int i = 0; i < list.Count; i++)
        {
            string prefix = list[i].isMe ? "<color=yellow>★ " : "  ";
            string suffix = list[i].isMe ? "</color>" : "";
            board += $"{prefix}{i + 1}. {list[i].playerName} - {list[i].score}{suffix}\n";
        }

        leaderboardText.text = board;
    }

    private void ShowRewardPanel(int rank, int reward)
    {
        if (rewardPanel != null) rewardPanel.SetActive(true);
        if (rewardMsgText != null)
        {
            rewardMsgText.text = $"Turnuva Bitti!\nSıralaman: {rank}.\nÖdülün: {reward} Coin";
        }
    }

    public void CloseRewardPanel()
    {
        if (rewardPanel != null) rewardPanel.SetActive(false);
    }
}
