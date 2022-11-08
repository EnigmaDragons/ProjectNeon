using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureConclusionState")]
public class AdventureConclusionState : ScriptableObject
{
    [SerializeField] private bool isVictorious;
    [SerializeField] private string endingTextTerm;
    [SerializeField] private RunStats runStats;
    [SerializeField] private HeroCharacter[] heroes;
    
    public bool IsVictorious => isVictorious;
    public string EndingStoryTextTerm => endingTextTerm;
    public RunStats Stats => runStats;
    public HeroCharacter[] Heroes => heroes;

    public void RecordFinishedGameAndCleanUp(bool playerWon, string storyTextTerm, RunStats stats, HeroCharacter[] runHeroes)
    {
        Set(playerWon, storyTextTerm, stats, runHeroes);
        CurrentGameData.Clear();
        CurrentProgressionData.Write(x =>
        {
            x.RunsFinished += 1;
            return x;
        });
    }
    
    private void Set(bool playerWon, string storyTextTerm, RunStats stats, HeroCharacter[] runHeroes)
    {
        isVictorious = playerWon;
        endingTextTerm = storyTextTerm;
        runStats = stats;
        heroes = runHeroes;
    }
}
