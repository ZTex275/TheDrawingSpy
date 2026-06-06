namespace DrawingSpy.Models;

/// <summary>Снимок игры для сохранения между запусками.</summary>
public class SavedGame
{
    public int TotalRounds { get; set; }

    public int CurrentRound { get; set; }

    public List<string> PlayerNames { get; set; } = [];

    public List<int[]> RoundScores { get; set; } = [];

    public List<int> SpyPerRound { get; set; } = [];

    public bool IsFinished => TotalRounds > 0 && RoundScores.Count >= TotalRounds;

    public bool CanContinue => CurrentRound > 0 && !IsFinished;
}
