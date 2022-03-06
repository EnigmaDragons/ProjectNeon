using UnityEngine;

public class NavigateToAcademyOrTitle : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    
    public void Execute()
    {
        if (CurrentAcademyData.Data.IsLicensedBenefactor)
            navigator.NavigateToTitleScreen();
        else
            navigator.NavigateToAcademyScene();
    }
}
