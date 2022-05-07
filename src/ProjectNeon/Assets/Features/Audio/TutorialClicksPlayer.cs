
using System;
using UnityEngine;

public class TutorialClicksPlayer : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string RightClick;
    [SerializeField, FMODUnity.EventRef] private string LeftClick;
    private void OnEnable()
    {
        Message.Subscribe<TutorialNextRequested>(OnLeftClick, this);
        Message.Subscribe<TutorialPreviousRequested>(OnRightClick, this);
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
