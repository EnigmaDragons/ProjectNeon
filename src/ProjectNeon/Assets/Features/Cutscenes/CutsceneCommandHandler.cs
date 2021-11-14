using UnityEngine;

public class CutsceneCommandHandler : OnMessage<StartCutsceneRequested>
{
    [SerializeField] private CurrentCutscene current;
    [SerializeField] private Navigator navigator;
    
    protected override void Execute(StartCutsceneRequested msg)
    {
        current.Init(msg.Cutscene);
        navigator.NavigateToCutsceneScene();
    }
}
