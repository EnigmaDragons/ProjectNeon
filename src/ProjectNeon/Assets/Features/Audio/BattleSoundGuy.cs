

using System;
using UnityEngine;

public class BattleSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEnemyDetalisShown;
    [SerializeField, FMODUnity.EventRef] private string OnCardPresented;
    [SerializeField, FMODUnity.EventRef] private string OnCardAiming;
    [SerializeField, FMODUnity.EventRef] private string OnTooltipHover;
    [SerializeField, FMODUnity.EventRef] private string OnCardDiscarded;
    [SerializeField, FMODUnity.EventRef] private string OnCardCanselled;
    [SerializeField, FMODUnity.EventRef] private string OnCardSelected;
    [SerializeField, FMODUnity.EventRef] private string OnCardDrawn;
    [SerializeField, FMODUnity.EventRef] private string OnCreditsChanged; //типа получили бабло
    [SerializeField, FMODUnity.EventRef] private string OnCardShuffled; //?
    private void OnEnable()
    {
        Message.Subscribe<ShowEnemySFX>(e => PlayOneShot(OnEnemyDetalisShown, e.UiSource), this);
        Message.Subscribe<CardHoverSFX>(OnCardPresentedSFX, this);
        Message.Subscribe<TargetChanged>(OnTargetChanged, this);
        Message.Subscribe<ShowTooltip>(OnTrashHovered, this);
        Message.Subscribe<CardDiscarded>(e => PlayOneShot(OnCardDiscarded, e.UiSource), this);
        Message.Subscribe<PlayerCardCanceled>(OnCardCanselledFUNC, this);
        Message.Subscribe<PlayerCardSelected>(OnCardSelectedFUNC, this);
        Message.Subscribe<PlayerCardDrawn>(OnCardDrawnFNUC, this);
        Message.Subscribe<PartyCreditsChanged>(OnCreditsChangedFUNC, this);
        Message.Subscribe<PlayerDeckShuffled>(OnCardShuffledFUNC, this);
        Message.Subscribe<SwappedCard>(e => PlayOneShot(OnCardDiscarded, e.UiSource), this);
       //Message.Subscribe<CharacterHoverChanged>(OnEnemyHoverFUNC, this);
    }

    /* private void OnEnemyHoverFUNC(CharacterHoverChanged msg)
     {
         if if(msg.Target.IsPresent)
             FMODUnity.RuntimeManager.PlayOneShot(OnCardAiming, Vector3.zero);
     }
    */

    private void OnCardShuffledFUNC(PlayerDeckShuffled msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardShuffled, Vector3.zero);
    }

    private void OnCreditsChangedFUNC(PartyCreditsChanged msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCreditsChanged, Vector3.zero);
    }

    private void OnCardDrawnFNUC(PlayerCardDrawn msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardDrawn, Vector3.zero);
    }

    private void OnCardSelectedFUNC(PlayerCardSelected msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardSelected, Vector3.zero);
    }

    private void OnCardCanselledFUNC(PlayerCardCanceled msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardCanselled, Vector3.zero);
    }

    private void OnTrashHovered(ShowTooltip msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnTooltipHover, Vector3.zero);
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
