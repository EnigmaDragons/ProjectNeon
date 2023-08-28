﻿using System;
using UnityEngine;

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

    protected override void Execute(NavigateToNextTutorialFlow msg) => Execute();

    public void Execute()
    {
        var d = CurrentAcademyData.Data;
        // Initial Settings
        if (!d.HasConfiguredSettings)
            navigator.NavigateToSettingsScene();
        // Old Academy
        else if (!d.IsLicensedBenefactor && !useNewTutorialFlow.Value)
            Message.Publish(new StartCutsceneRequested(cutscene, Maybe<Action>.Present(() => navigator.NavigateToAcademyScene())));
        // Continue Tutorial Adventure
        else if (!d.IsLicensedBenefactor && useNewTutorialFlow.Value && io.HasSavedGame)
        {
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
            Message.Publish(new StartAdventureV5Requested(tutorialAdventure, Maybe<BaseHero[]>.Missing(), Maybe<Difficulty>.Missing()));
        // Welcome To Metroplex Cutscene
        else if (!d.HasCompletedWelcomeToMetroplexCutscene)
            entranceCutsceneStarter.Execute();
        // Main Menu
        else
            navigator.NavigateToTitleScreen();
    }
}