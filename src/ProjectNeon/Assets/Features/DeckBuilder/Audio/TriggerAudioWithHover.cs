using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudioWithHover : MonoBehaviour
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
        if (!string.IsNullOrWhiteSpace(sound))
            FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);
    }
}
