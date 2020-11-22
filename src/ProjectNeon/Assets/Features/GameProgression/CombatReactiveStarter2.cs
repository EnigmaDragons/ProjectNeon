using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatReactiveStarter2 : OnMessage<EnterRandomCombat, EnterBossBattle>
{
    [SerializeField] private AdventureProgress2 progress;
    [SerializeField] private BattleState battleState;
    
    protected override void Execute(EnterRandomCombat msg)
    {
        Log.Info("Setting Up Random Encounter");
        battleState.SetNextBattleground(progress.CurrentStage.Battleground);
        battleState.SetNextEncounter(progress.CurrentStage.EncounterBuilder.Generate(progress.CurrentPowerLevel));
        SceneManager.LoadScene("BattleSceneV2");
    }

    protected override void Execute(EnterBossBattle msg)
    {
        Log.Info("Setting Up Boss Battle");
        battleState.SetNextBattleground(progress.CurrentStage.BossBattlefield);
        battleState.SetNextEncounter(progress.CurrentStage.BossEnemies);
        SceneManager.LoadScene("BattleSceneV2");
    }
}