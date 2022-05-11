using System;
using UnityEngine;

public class NavigateToSettingsOrAcedemyOrTitleOrWelcomeCutscene : MonoBehaviour
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private PlayEnterMetroplexZeroCutscene entranceCutsceneStarter;
    [SerializeField] private BoolReference useNewTutorialFlow;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Adventure tutorialAdventure;
    
    public void Execute()
    {
        var d = CurrentAcademyData.Data;
        if (!d.HasConfiguredSettings)
            navigator.NavigateToSettingsScene();
        else if (!d.IsLicensedBenefactor && !useNewTutorialFlow.Value)
            Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() => navigator.NavigateToAcademyScene())));
        else if (!d.IsLicensedBenefactor && useNewTutorialFlow.Value)
            Message.Publish(new StartAdventureV5Requested(tutorialAdventure));
        else if (!d.HasCompletedWelcomeToMetroplexCutscene)
            entranceCutsceneStarter.Execute();
        else
            navigator.NavigateToTitleScreen();
    }
}