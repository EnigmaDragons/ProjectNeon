using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatReactiveStarter2 : OnMessage<EnterRandomCombat, EnterRandomEliteCombat, EnterBossBattle, EnterSpecificBattle>
{
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private BattleState battleState;
    [SerializeField] private EventPublisher eventPublisher;
    
    protected override void Execute(EnterRandomCombat msg)
    {
        Log.Info("Setting Up Random Encounter");
        battleState.SetNextBattleground(progress.CurrentStage.Battleground);
        battleState.SetNextEncounter(progress.CurrentStage.EncounterBuilder.Generate(progress.CurrentPowerLevel));
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterRandomEliteCombat msg)
    {
        Log.Info("Setting Up Random Elite Encounter");
        battleState.SetNextBattleground(progress.CurrentStage.Battleground);
        battleState.SetNextEncounter(progress.CurrentStage.EliteEncounterBuilder.Generate(progress.CurrentElitePowerLevel), isElite: true);
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterBossBattle msg)
    {
        Log.Info("Setting Up Boss Battle");
        battleState.SetNextBattleground(progress.CurrentStage.BossBattlefield);
        battleState.SetNextEncounter(progress.CurrentStage.BossEnemies.Select(x => x.GetEnemy(progress.Stage)));
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }

    protected override void Execute(EnterSpecificBattle msg)
    {
        Log.Info("Setting Up Specific Battle");
        battleState.SetNextBattleground(msg.BattleField);
        battleState.SetNextEncounter(msg.Enemies, msg.IsElite);
        eventPublisher.ActivatePartyDetailsWizardFlow();
    }
}