using UnityEngine;

public class FModButtonSounds : OnMessage<NavigateToSceneRequested>
{
    [SerializeField, FMODUnity.EventRef] private string bigClickSound;
    [SerializeField, FMODUnity.EventRef] private string standardClickSound;
    [SerializeField] private bool useBigClickSound;
    [SerializeField, FMODUnity.EventRef] private string hoverEnterSound;
    [SerializeField, FMODUnity.EventRef] private string hoverExitSound;

    public void OnClick() => Play(useBigClickSound ? bigClickSound : standardClickSound, true);
    public void OnHoverEnter() => Play(hoverEnterSound);
    public void OnHoverExit() => Play(hoverExitSound);

    public void SetAsPrimary() => useBigClickSound = true;
    
    private void Play(string sound, bool playEvenIfNotEnabled = false)
    {
        if ((enabled || playEvenIfNotEnabled) && !string.IsNullOrWhiteSpace(sound))
            FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    protected override void Execute(NavigateToSceneRequested msg) => enabled = false;
}
