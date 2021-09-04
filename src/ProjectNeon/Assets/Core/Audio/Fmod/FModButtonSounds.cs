using UnityEngine;

public class FModButtonSounds : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string bigClickSound;
    [SerializeField, FMODUnity.EventRef] private string standardClickSound;
    [SerializeField] private bool useBigClickSound;
    [SerializeField, FMODUnity.EventRef] private string hoverEnterSound;
    [SerializeField, FMODUnity.EventRef] private string hoverExitSound;

    public void OnClick() => Play(useBigClickSound ? bigClickSound : standardClickSound);
    public void OnHoverEnter() => Play(hoverEnterSound);
    public void OnHoverExit() => Play(hoverExitSound);

    public void SetAsPrimary() => useBigClickSound = true;
    
    private void Play(string sound)
    {
        if (!string.IsNullOrWhiteSpace(sound))
            FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }
}