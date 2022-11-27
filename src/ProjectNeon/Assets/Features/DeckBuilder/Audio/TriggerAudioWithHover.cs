using UnityEngine;

public class TriggerAudioWithHover : OnMessage<NavigateToSceneRequested>
{
    [FMODUnity.EventRef]
    public string Event;
    public bool PlayOnAwake;
    public bool PlayOnDestroy;
    [SerializeField, FMODUnity.EventRef] private string hoverEnterSound;

    public void OnHoverEnter() => Play(hoverEnterSound);

    public void PlayOneShot()
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(Event, gameObject);
    }

    private void Start()
    {
        if (PlayOnAwake)
            PlayOneShot();
    }

    private void OnDestroy()
    {
        if (PlayOnDestroy)
            PlayOneShot();
    }

    private void Play(string sound)
    {
        if (enabled && !string.IsNullOrWhiteSpace(sound))
            FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }

    protected override void Execute(NavigateToSceneRequested msg) => enabled = false;
}
