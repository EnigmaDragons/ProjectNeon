using System;
using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureConclusionState")]
public class AdventureConclusionState : ScriptableObject
{
    private static bool isVictorious;
    private static string endingTextTerm;
    private static RunStats runStats;
    private static Hero[] heroes;
    
    public bool IsVictorious => isVictorious;
    public string EndingStoryTextTerm => endingTextTerm;
    public RunStats Stats => runStats;
    public Hero[] Heroes => heroes;

    public static void Clear()
    {
        isVictorious = false;
        endingTextTerm = "";
        runStats = new RunStats();
        heroes = Array.Empty<Hero>();
    }
    
    public void RecordFinishedGameAndCleanUp(bool playerWon, string storyTextTerm, RunStats stats, Hero[] runHeroes)
    {
        Set(playerWon, storyTextTerm, stats, runHeroes);
        CurrentGameData.Clear();
        CurrentProgressionData.Write(x =>
        {
            x.RunsFinished += 1;
            return x;
        });
    }
    
    private void Set(bool playerWon, string storyTextTerm, RunStats stats, Hero[] runHeroes)
    {
        Log.Info($"Adventure Conclusion - Set {playerWon} {storyTextTerm}");
        isVictorious = playerWon;
        endingTextTerm = storyTextTerm;
        runStats = stats;
        heroes = runHeroes;
    }
}
