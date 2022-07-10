using UnityEngine;

public class ProgressionProgress : MonoBehaviour
{
    [SerializeField] private Library library;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private Adventure firstStoryAdventure;
    
    public bool DraftModeIsUnlocked()
    {
        if (firstStoryAdventure == null)
            return false;
        
        var completedAcademy = CurrentAcademyData.Data.IsLicensedBenefactor;
        var progress = CurrentProgressionData.Data;
        return completedAcademy && progress.Completed(firstStoryAdventure.Id);
    }
}
