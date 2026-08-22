using UnityEngine;
public static class ScoreEvents
{
    public static event System.Action<int> OnScoreChanged;
    public static event System.Action<int, bool> OnGameOver;
    public static void RaiseScoreChanged(int score) => OnScoreChanged?.Invoke(score);
    public static void RaiseGameOver(int finalScore, bool isNewHighScore) => OnGameOver?.Invoke(finalScore, isNewHighScore);
}