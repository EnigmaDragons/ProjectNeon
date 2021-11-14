using FMODUnity;
using UnityEngine;

[CreateAssetMenu(menuName = "SFX/FmodPlayableUiSound")]
public class FmodPlayableUiSound : PlayableUiSound
{
    [EventRef] public string sfx;

    public override void Play() => Play(Vector3.zero);
    public override void Play(Vector3 position)
    {
        if (!string.IsNullOrWhiteSpace(sfx))   
            RuntimeManager.PlayOneShot(sfx, position);
    }
}
