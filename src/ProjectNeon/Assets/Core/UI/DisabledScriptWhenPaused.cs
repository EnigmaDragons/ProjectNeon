using UnityEngine;

public sealed class DisabledScriptWhenPaused : OnMessage<GamePaused, GameContinued>
{
    [SerializeField] private MonoBehaviour targetScript;
    
    protected override void Execute(GamePaused msg) => targetScript.enabled = false;
    protected override void Execute(GameContinued msg) => targetScript.enabled = true;
}

