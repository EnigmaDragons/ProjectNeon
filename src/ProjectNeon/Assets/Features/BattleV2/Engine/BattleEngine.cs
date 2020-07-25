using System.Collections;
using UnityEngine;

public class BattleEngine : MonoBehaviour
{
    [SerializeField] private BattleSetupV2 setup;
    [SerializeField] private BattleCommandPhase commandPhase;
    [SerializeField] private bool logProcessSteps;
    [SerializeField] private bool setupOnStart;
    [SerializeField, ReadOnly] private BattleV2Phase phase = BattleV2Phase.NotBegun;
    
    public void Start()
    {
        if (setupOnStart)
            Setup();
    }
    
    public void Setup() => StartCoroutine(ExecuteSetupAsync());

    private void BeginCommandPhase()
    {
        BeginPhase(BattleV2Phase.Command);
        commandPhase.Begin();
        Message.Publish(new TurnStarted());
    }
    
    private IEnumerator ExecuteSetupAsync()
    {
        BeginPhase(BattleV2Phase.Setup);
        yield return setup.Execute();
        BeginCommandPhase();
    }

    private void BeginPhase(BattleV2Phase newPhase)
    {
        if (phase != BattleV2Phase.NotBegun)
            LogProcessStep($"Finished {phase} Phase");
        LogProcessStep($"Beginning {newPhase} Phase");
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
        Command = 40,
        Resolution = 50,
        Wrapup = 60,
        Finished = 80
    }
}
