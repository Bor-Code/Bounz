using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TournamentPlayer
{
    public string playerName;
    public int score;
    public bool isMe;
}

public class TournamentManager : MonoBehaviour
{
    public static TournamentManager Instance { get; private set; }

    private const string TournamentEndKey = "TournamentEndDate";
    private const string TournamentPlayersKey = "TournamentPlayers_";
    
    [SerializeField] private int tournamentDurationDays = 7;
    [SerializeField] private int top1Reward = 5000;
    [SerializeField] private int top2Reward = 2000;
    [SerializeField] private int top3Reward = 1000;

    public List<TournamentPlayer> CurrentLeaderboard { get; private set; } = new List<TournamentPlayer>();
    public DateTime EndDate { get; private set; }

    public event Action OnLeaderboardUpdated;
    public event Action<int, int> OnTournamentRewardClaimed; // Rank, Reward

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeTournament();
    }

    private void OnEnable()
    {
        ScoreEvents.OnGameOver += HandleGameOver;
    }

    private void OnDisable()
    {
        ScoreEvents.OnGameOver -= HandleGameOver;
    }

    private void InitializeTournament()
    {
        string endDateStr = PlayerPrefs.GetString(TournamentEndKey, "");
        if (string.IsNullOrEmpty(endDateStr))
        {
            StartNewTournament();
        }
        else
        {
            if (long.TryParse(endDateStr, out long endDateBinary))
            {
                EndDate = DateTime.FromBinary(endDateBinary);
                if (DateTime.Now >= EndDate)
                {
                    EndTournamentAndDistributeRewards();
                    StartNewTournament();
                }
                else
                {
                    LoadLeaderboard();
                }
            }
            else
            {
                StartNewTournament();
            }
        }
    }

    private void StartNewTournament()
    {
        EndDate = DateTime.Now.AddDays(tournamentDurationDays);
        PlayerPrefs.SetString(TournamentEndKey, EndDate.ToBinary().ToString());
        
        GenerateFakeLeaderboard();
        SaveLeaderboard();
    }

    private void GenerateFakeLeaderboard()
    {
        CurrentLeaderboard.Clear();
        string[] names = { "Alex", "ProGamer", "Shadow", "Bouncer99", "Ninja", "Kratos", "Zeus", "Ghost", "Speedy", "JumpKing" };
        
        for (int i = 0; i < 15; i++)
        {
            CurrentLeaderboard.Add(new TournamentPlayer
            {
                playerName = names[UnityEngine.Random.Range(0, names.Length)] + UnityEngine.Random.Range(10, 999),
                score = UnityEngine.Random.Range(50, 2000),
                isMe = false
            });
        }

        int myHighScore = SaveManager.Instance != null ? SaveManager.Instance.CurrentSave.highScore : 0;
        CurrentLeaderboard.Add(new TournamentPlayer { playerName = "Sen (You)", score = myHighScore, isMe = true });
        
        SortLeaderboard();
    }

    private void SortLeaderboard()
    {
        CurrentLeaderboard = CurrentLeaderboard.OrderByDescending(p => p.score).ToList();
        OnLeaderboardUpdated?.Invoke();
    }

    private void LoadLeaderboard()
    {
        CurrentLeaderboard.Clear();
        int count = PlayerPrefs.GetInt(TournamentPlayersKey + "Count", 0);
        if (count == 0)
        {
            GenerateFakeLeaderboard();
            return;
        }

        for (int i = 0; i < count; i++)
        {
            CurrentLeaderboard.Add(new TournamentPlayer
            {
                playerName = PlayerPrefs.GetString(TournamentPlayersKey + "Name_" + i),
                score = PlayerPrefs.GetInt(TournamentPlayersKey + "Score_" + i),
                isMe = PlayerPrefs.GetInt(TournamentPlayersKey + "IsMe_" + i) == 1
            });
        }
        SortLeaderboard();
    }

    private void SaveLeaderboard()
    {
        PlayerPrefs.SetInt(TournamentPlayersKey + "Count", CurrentLeaderboard.Count);
        for (int i = 0; i < CurrentLeaderboard.Count; i++)
        {
            PlayerPrefs.SetString(TournamentPlayersKey + "Name_" + i, CurrentLeaderboard[i].playerName);
            PlayerPrefs.SetInt(TournamentPlayersKey + "Score_" + i, CurrentLeaderboard[i].score);
            PlayerPrefs.SetInt(TournamentPlayersKey + "IsMe_" + i, CurrentLeaderboard[i].isMe ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void HandleGameOver(int finalScore, bool isNewHighScore)
    {
        if (isNewHighScore)
        {
            var me = CurrentLeaderboard.FirstOrDefault(p => p.isMe);
            if (me != null && finalScore > me.score)
            {
                me.score = finalScore;
                SortLeaderboard();
                SaveLeaderboard();
            }
        }
    }

    private void EndTournamentAndDistributeRewards()
    {
        LoadLeaderboard(); 
        int myRank = CurrentLeaderboard.FindIndex(p => p.isMe) + 1;
        
        int reward = 0;
        if (myRank == 1) reward = top1Reward;
        else if (myRank == 2) reward = top2Reward;
        else if (myRank == 3) reward = top3Reward;

        if (reward > 0 && SaveManager.Instance != null)
        {
            SaveManager.Instance.CurrentSave.totalScore += reward;
            SaveManager.Instance.SaveGame();
            OnTournamentRewardClaimed?.Invoke(myRank, reward);
        }
    }
}
