using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureConclusionState")]
public class AdventureConclusionState : ScriptableObject
{
    [SerializeField] private bool isVictorious;
    [SerializeField] private string endingText;
    [SerializeField] private RunStats runStats;
    [SerializeField] private HeroCharacter[] heroes;
    
    public bool IsVictorious => isVictorious;
    public string EndingStoryText => endingText;
    public RunStats Stats => runStats;
    public HeroCharacter[] Heroes => heroes;

    public void RecordFinishedGameAndCleanUp(bool playerWon, string storyText, RunStats stats, HeroCharacter[] runHeroes)
    {
        Set(playerWon, storyText, stats, runHeroes);
        CurrentGameData.Clear();
        CurrentProgressionData.Write(x =>
        {
            x.RunsFinished += 1;
            return x;
        });
    }
    
    private void Set(bool playerWon, string storyText, RunStats stats, HeroCharacter[] runHeroes)
    {
        isVictorious = playerWon;
        endingText = storyText;
        runStats = stats;
        heroes = runHeroes;
    }
}
