
using System;
using UnityEngine;

public class BattleSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEnemyDetalisShown;
    [SerializeField, FMODUnity.EventRef] private string OnCardPresented;
    [SerializeField, FMODUnity.EventRef] private string OnCardAiming;

    private void OnEnable()
    {
        Message.Subscribe<ShowEnemySFX>(e => PlayOneShot(OnEnemyDetalisShown, e.UiSource), this);
        Message.Subscribe<CardHoverSFX>(OnCardPresentedSFX, this);
        Message.Subscribe<TargetChanged>(OnTargetChanged, this);
    }

    private void OnTargetChanged(TargetChanged msg)
    {
        if(msg.Target.IsPresent)
            FMODUnity.RuntimeManager.PlayOneShot(OnCardAiming, Vector3.zero);
    }

    private void OnCardPresentedSFX(CardHoverSFX msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardPresented, Vector3.zero);
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
