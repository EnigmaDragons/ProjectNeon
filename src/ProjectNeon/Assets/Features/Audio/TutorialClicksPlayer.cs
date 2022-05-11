
using System;
using UnityEngine;

public class TutorialClicksPlayer : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string RightClick;
    [SerializeField, FMODUnity.EventRef] private string LeftClick;
    [SerializeField, FMODUnity.EventRef] private string HeroNameHover;
    
    private void OnEnable()
    {
        Message.Subscribe<TutorialNextRequested>(OnLeftClick, this);
        Message.Subscribe<TutorialPreviousRequested>(OnRightClick, this);
        Message.Subscribe<ShowTooltip>(OnHeroNameHover, this);
        
    }

  

    private void OnHeroNameHover(ShowTooltip msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(HeroNameHover, Vector3.zero);
    }

    private void OnRightClick(TutorialPreviousRequested msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(RightClick, Vector3.zero);
    }

    private void OnLeftClick(TutorialNextRequested msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(LeftClick, Vector3.zero);
    }

    private bool debuggingLoggingEnabled = false;
    private void OnDisable()
    {
        Message.Unsubscribe(this);
    }
    private void PlayOneShot(string eventName, Transform uiSource)
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
    private void DebugLog(string msg)
    {
        if (debuggingLoggingEnabled)
            Log.Info(msg);
    }
}
