using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene : OnMessage<NavigateToNextTutorialFlow>
{
    [SerializeField] private Navigator navigator;
    [SerializeField] private Cutscene cutscene;
    [SerializeField] private PlayEnterMetroplexZeroCutscene entranceCutsceneStarter;
    [SerializeField] private BoolReference useNewTutorialFlow;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private Adventure tutorialAdventure;
    [SerializeField] private SaveLoadSystem io;
    [SerializeField] private AdventureProgressV5 adventureProgress5;

    private bool _triggered;
    
    protected override void Execute(NavigateToNextTutorialFlow msg) => Execute();

    protected override void AfterEnable()
    {
        this.ExecuteAfterDelay(20, () =>
        {
            if (SceneManager.GetActiveScene().name != "VideoLogoScene")
                return;
            if (this != null && !_triggered)
                Log.Error("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene did not navigate away from the Video Logo Scene in 20 seconds.");
        });
    }

    public void Execute()
    {
        _triggered = true;
        Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - Execute");
        var d = CurrentAcademyData.Data;
        // Initial Settings
        if (!d.HasConfiguredSettings)
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - NavigateToSettingsScene");
            navigator.NavigateToSettingsScene();
        }
        // Old Academy
        else if (!d.IsLicensedBenefactor && !useNewTutorialFlow.Value)
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - Old Academy");
            Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() => navigator.NavigateToAcademyScene())));
        }
        // Continue Tutorial Adventure
        else if (!d.IsLicensedBenefactor && useNewTutorialFlow.Value && io.HasSavedGame)
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - Continue Tutorial");
            try
            {
                var phase = io.LoadSavedGame();
                if (phase != CurrentGamePhase.LoadError && phase != CurrentGamePhase.NotStarted)
                    GameStarter.Navigate(navigator, adventureProgress5);
                else 
                    navigator.NavigateToTitleScreen();
            }
            catch (Exception e)
            {
                Log.Error(e);
                navigator.NavigateToTitleScreen();
            }
        }
        // Start Tutorial Adventure
        else if (!d.IsLicensedBenefactor && useNewTutorialFlow.Value)
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - Start Tutorial");
            Message.Publish(new StartAdventureV5Requested(tutorialAdventure, Maybe<BaseHero[]>.Missing(), Maybe<Difficulty>.Missing()));
        }
        // Welcome To Metroplex Cutscene
        else if (!d.HasCompletedWelcomeToMetroplexCutscene)
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - WelcomeToMetroplexCutscene");
            entranceCutsceneStarter.Execute();
        }
        // Main Menu
        else
        {
            Log.Info("NavigateToSettingsOrAcademyOrTitleOrWelcomeCutscene - NavigateToTitleScreen");
            navigator.NavigateToTitleScreen();
        }
    }
}