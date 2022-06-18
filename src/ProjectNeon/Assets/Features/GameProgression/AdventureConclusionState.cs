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
    
    public void Set(bool playerWon, string storyText, RunStats stats, HeroCharacter[] runHeroes)
    {
        isVictorious = playerWon;
        endingText = storyText;
        runStats = stats;
        heroes = runHeroes;
    }
}
