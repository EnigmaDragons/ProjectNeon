using UnityEngine;

public class CutsceneCommandHandler : OnMessage<StartCutsceneRequested, SetStartBattleCutsceneRequested>
{
    [SerializeField] private CurrentCutscene current;
    [SerializeField] private Navigator navigator;
    
    protected override void Execute(StartCutsceneRequested msg)
    {
        Log.Info("Received Start Cutscene Requested Message");
        current.Init(msg.Cutscene,  msg.OnFinished);
        navigator.NavigateToCutsceneScene();
    }

    protected override void Execute(SetStartBattleCutsceneRequested msg)
    {
        current.InitStartBattle(msg.Cutscene);
    }
}
 