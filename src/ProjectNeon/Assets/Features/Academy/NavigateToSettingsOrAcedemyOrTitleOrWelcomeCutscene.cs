using System;
using UnityEngine;

public class NavigateToSettingsOrAcedemyOrTitleOrWelcomeCutscene : OnMessage<NavigateToNextTutorialFlow>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private PlayEnterMetroplexZeroCutscene entranceCutsceneStarter;
    [SerializeField] private BoolReference useNewTutorialFlow;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private SaveLoadSystem io;

    protected override void Execute(NavigateToNextTutorialFlow msg) => Execute();

    public void Execute()
    {
        var d = CurrentAcademyData.Data;
        if (!d.HasConfiguredSettings)
            navigator.NavigateToSettingsScene();
        else if (!d.IsLicensedBenefactor && !useNewTutorialFlow.Value)
            Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() => navigator.NavigateToAcademyScene())));
        else if (!d.IsLicensedBenefactor 
             && useNewTutorialFlow.Value 
             && CurrentGameData.HasActiveGame 
             && adventure.Adventure != null 
             && adventure.Adventure.Id == tutorialAdventure.Id && io.HasSavedGame)
        {
            io.LoadSavedGame();
            Message.Publish(new GameLoaded());
            navigator.NavigateToGameSceneV5();
        }
        else if (!d.IsLicensedBenefactor && useNewTutorialFlow.Value)
            Message.Publish(new StartAdventureV5Requested(tutorialAdventure));
        else if (!d.HasCompletedWelcomeToMetroplexCutscene)
            entranceCutsceneStarter.Execute();
        else
            navigator.NavigateToTitleScreen();
    }
}