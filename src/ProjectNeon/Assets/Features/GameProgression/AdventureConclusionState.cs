using UnityEngine;

[CreateAssetMenu(menuName = "OnlyOnce/CurrentAdventureConclusionState")]
public class AdventureConclusionState : ScriptableObject
{
    [SerializeField] private bool isVictorious;
    [SerializeField] private string endingText;

    public bool IsVictorious => isVictorious;
    public string EndingStoryText => endingText;
    
    public void Set(bool playerWon, string storyText)
    {
        isVictorious = playerWon;
        endingText = storyText;
    }
}
