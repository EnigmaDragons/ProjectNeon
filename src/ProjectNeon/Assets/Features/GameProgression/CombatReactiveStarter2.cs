using System.Linq;
using Features.GameProgression;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatReactiveStarter2 : OnMessage<EnterRandomCombat, EnterRandomEliteCombat, EnterBossBattle, EnterSpecificBattle>
{
    [SerializeField] private CurrentAdventureProgress progress;
    [SerializeField] private BattleState battleState;
    [SerializeField] private EventPublisher eventPublisher;
    [SerializeField] private Navigator navigator;
    
    protected override void Execute(EnterRandomCombat msg)
    {
        Log.Info("Setting Up Random Encounter");
        battleState.SetNextBattleground(progress.AdventureProgress.Stage.BattlegroundForSegment(progress.AdventureProgress.CurrentStageProgress));
        battleState.SetNextEncounter(progress.AdventureProgress.Stage.EncounterBuilder.Generate(progress.AdventureProgress.CurrentPowerLevel, progress.AdventureProgress.CurrentChapterNumber));
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterRandomEliteCombat msg)
    {
        Log.Info("Setting Up Random Elite Encounter");
        battleState.SetNextBattleground(progress.AdventureProgress.Stage.BattlegroundForSegment(progress.AdventureProgress.CurrentStageProgress));
        battleState.SetNextEncounter(progress.AdventureProgress.Stage.EliteEncounterBuilder.Generate(progress.AdventureProgress.CurrentElitePowerLevel, progress.AdventureProgress.CurrentChapterNumber), isElite: true);
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterBossBattle msg)
    {
        Log.Info("Setting Up Boss Battle");
        battleState.SetNextBattleground(progress.AdventureProgress.Stage.BossBattlefield);
        battleState.SetNextEncounter(progress.AdventureProgress.Stage.BossEnemies.Select(x => x.ForStage(progress.AdventureProgress.CurrentChapterNumber)));
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterSpecificBattle msg)
    {
        Log.Info("Setting Up Specific Battle");
        battleState.SetNextBattleground(msg.BattleField);
        battleState.SetNextEncounter(msg.Enemies, msg.IsElite, msg.IsStoryEventCombat);
        if (msg.IsTutorial)
        {
            battleState.DontShuffleNextBattle = true;
            navigator.NavigateToBattleScene();   
        }
        else
        {
            battleState.DontShuffleNextBattle = false;
            eventPublisher.ActivatePartyDetailsWizardFlow();
        }
    }
}