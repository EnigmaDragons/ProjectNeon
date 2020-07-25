using System.Collections;
using UnityEngine;

public class BattleEngine : MonoBehaviour
{
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private bool logProcessSteps;
    [SerializeField, ReadOnly] private BattleV2Phase phase = BattleV2Phase.NotBegun;

    public void Setup() => StartCoroutine(ExecuteSetupAsync());

    private IEnumerator ExecuteSetupAsync()
    {
        BeginPhase(BattleV2Phase.Setup);
        yield return setup.Execute();
        BeginPhase(BattleV2Phase.Action);
    }

    private void BeginPhase(BattleV2Phase newPhase)
    {
        LogProcessStep($"Finished {phase}");
        LogProcessStep($"Beginning {newPhase}");
        phase = newPhase;
    }
    
    private void LogProcessStep(string message)
    {
        if (logProcessSteps)
            BattleLog.Write(message);
    }
    
    public enum BattleV2Phase
    {
        NotBegun = 0,
        Setup = 20,
        Action = 40,
        Wrapup = 60,
        Finished = 80
    } 
}
