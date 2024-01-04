﻿using System;
using UnityEngine;

public class SkipTutorialBattleButton : OnMessage<BattleFinished>, ILocalizeTerms
{
    [SerializeField] private LocalizedCommandButton button;
    [SerializeField] private BattleState battleState;
    [SerializeField] private CurrentAdventure adventure;
    [SerializeField] private EventPublisher eventPublisher;

    private string Term => "Tutorials/SkipTutorial";
    
    private void Start()
    {
        try
        {
            if (adventure.Adventure == null)
            {
                Log.Warn("Disabling UI - SkipTutorialBattleButton because no adventure is set");
                gameObject.SetActive(false);
                return;
            }

            button.gameObject.SetActive(adventure.Adventure.Id == AdventureIds.TutorialAdventureId || battleState.IsSingleTutorialBattle);
            if (battleState.IsSingleTutorialBattle)
                button.InitTerm(Term, () => eventPublisher.WinBattle());
            else if (adventure.Adventure.Id == AdventureIds.TutorialAdventureId)
                button.InitTerm(Term, () => Message.Publish(new ShowLocalizedDialog
                {
                    PrimaryAction = () => eventPublisher.WinBattle(),
                    PrimaryButtonTerm = DialogTerms.OptionSkipBattle,
                    SecondaryAction = () => { },
                    SecondaryButtonTerm = DialogTerms.OptionOops,
                    PromptTerm = DialogTerms.SkipTutorialBattleWarning
                }));
        }
        catch (Exception e)
        {
            gameObject.SetActive(false);
            Log.NonCrashingError(e);
        }
    }

    public string[] GetLocalizeTerms()
        => new[] {Term, DialogTerms.OptionSkipBattle, DialogTerms.OptionOops, DialogTerms.SkipTutorialBattleWarning};

    protected override void Execute(BattleFinished msg)
        => button.gameObject.SetActive(false);
}