using UnityEngine;

public class FmodCheatAcceptedSound : OnMessage<CheatAcceptedSuccessfully>
{
    [SerializeField, FMODUnity.EventRef] private string sound;
    
    protected override void Execute(CheatAcceptedSuccessfully msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(sound, Vector3.zero);
    }
}
