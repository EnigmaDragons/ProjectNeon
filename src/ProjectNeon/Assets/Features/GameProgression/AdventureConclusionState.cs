using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureConclusionState")]
public class AdventureConclusionState : ScriptableObject
{
    [SerializeField] private bool isVictorious;
    [SerializeField] private string endingText;
    [SerializeField] private RunStats runStats;
    
    public bool IsVictorious => isVictorious;
    public string EndingStoryText => endingText;
    public RunStats Stats => runStats;
    
    public void Set(bool playerWon, string storyText, RunStats stats)
    {
        isVictorious = playerWon;
        endingText = storyText;
        runStats = stats;
    }
}
