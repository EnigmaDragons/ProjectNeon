using System;
using UnityEngine;

public class NavigateToAcademyOrTitle : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Cutscene cutscene;
    
    public void Execute()
    {
        if (CurrentAcademyData.Data.IsLicensedBenefactor)
            navigator.NavigateToTitleScreen();
        else
            Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() => navigator.NavigateToAcademyScene())));
    }
}
