using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;

public class BattleSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEnemyDetalisShown;
    [SerializeField, FMODUnity.EventRef] private string OnEnemyDetalisHidden;
    [SerializeField, FMODUnity.EventRef] private string OnCardPresented;
    [SerializeField, FMODUnity.EventRef] private string OnCardHoverExit;
    private FMOD.Studio.EventInstance TrashShaking;
    private FMOD.Studio.EventInstance CardCycle;
    private FMOD.Studio.EventInstance BattleWonStinger;
    [SerializeField, FMODUnity.EventRef] private string OnCardAiming;
    [SerializeField, FMODUnity.EventRef] private string OnCardAimingLONG;
    [SerializeField, FMODUnity.EventRef] private string OnTooltipHover;
    [SerializeField, FMODUnity.EventRef] private string OnCardRecycled;
    [SerializeField, FMODUnity.EventRef] private string OnCardDiscarded;
    [SerializeField, FMODUnity.EventRef] private string OnCardSelected;
    [SerializeField, FMODUnity.EventRef] private string OnCardDrawn;
    [SerializeField, FMODUnity.EventRef] private string OnCreditsChanged; 
    [SerializeField, FMODUnity.EventRef] private string OnCardShuffled; 
    [SerializeField, FMODUnity.EventRef] private string OnCardReZone;
    [SerializeField, FMODUnity.EventRef] private string OnCardSwapped;
    [SerializeField, FMODUnity.EventRef] private string OnCardRightClick;
    [SerializeField, FMODUnity.EventRef] private string OnCardRightClickBack;
    
    private void OnEnable()
    {
        Message.Subscribe<ShowEnemySFX>(e => PlayOneShot(OnEnemyDetalisShown, e.UiSource), this);
        Message.Subscribe<HideEnemyDetails>(OnEnemyDetailsHiddenSFX, this);
        Message.Subscribe<CardHoverSFX>(OnCardPresentedSFX, this);
        Message.Subscribe<CardHoverExitSFX>(e => PlayOneShot(OnCardHoverExit, e.UiSource), this);
        Message.Subscribe<TargetChanged>(OnTargetChanged, this);
        Message.Subscribe<ShowTooltip>(OnTrashHovered, this);
        Message.Subscribe<CardDiscarded>(e => PlayOneShot(OnCardRecycled, e.UiSource), this);
        Message.Subscribe<PlayerCardCanceled>(OnCardCancelled, this);
        Message.Subscribe<PlayerCardSelected>(OnCardSelectedFUNC, this);
        Message.Subscribe<PlayerCardDrawn>(OnCardDrawnFNUC, this);
        Message.Subscribe<PartyCreditsChanged>(OnCreditsChangedFUNC, this);
        Message.Subscribe<PlayerDeckShuffled>(OnCardShuffledFUNC, this);
        Message.Subscribe<SwappedCard>(e => PlayOneShot(OnCardSwapped, e.UiSource), this);
        Message.Subscribe<CharacterHoverChanged>(OnEnemyHoverFUNC, this);
        Message.Subscribe<CardResolutionStarted>(OnCardReFUNC, this);
        Message.Subscribe<TweenMovementRequested>(OnCardRightClickFUNC, this);
        Message.Subscribe<SnapBackTweenRequested>(OnCardRightClickBackFUNC, this);
        Message.Subscribe<HoverEntered>(OnHoverEntered, this);
        Message.Subscribe<HoverExited>(OnHoverExited, this);
        Message.Subscribe<WinBattleWithRewards>(_ => OnBattleWon(), this);
        Message.Subscribe<BattleRewardsStarted>(_ => OnBattleWon(), this);
        Message.Subscribe<NavigateToSceneRequested>(OnBattleWonFadeFUNC, this);
    }

    private void OnBattleWon()
    {        
        if (!BattleWonStinger.isValid())
            BattleWonStinger = FMODUnity.RuntimeManager.CreateInstance("event:/BattleScene/BATTLE_WON_STINGER");
        if (BattleWonStinger.getPlaybackState(out var pb) != RESULT.OK || pb != PLAYBACK_STATE.PLAYING)
            BattleWonStinger.start();
    }
    
    private void OnBattleWonFadeFUNC(NavigateToSceneRequested msg)
    {
        BattleWonStinger.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        BattleWonStinger.release();
    }
        
    private void OnHoverEntered(HoverEntered msg)
    {
        if (msg.ElementName == "CycleCardDropTarget")
        {
            CardCycle = FMODUnity.RuntimeManager.CreateInstance("event:/BattleScene/CARD_CYCLE");
            CardCycle.start();
        }
        if (msg.ElementName == "DiscardDropTarget")
        {
            TrashShaking = FMODUnity.RuntimeManager.CreateInstance("event:/BattleScene/TRASH_SHAKING");
            TrashShaking.start();   
        }
    }
    
    private void OnHoverExited(HoverExited msg)
    {
        if (msg.ElementName == "CycleCardDropTarget")
        {
            CardCycle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            CardCycle.release();
        }
        if (msg.ElementName == "DiscardDropTarget")
        {
            TrashShaking.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            TrashShaking.release();
        }
    }
    
    private void OnEnemyDetailsHiddenSFX(HideEnemyDetails msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnEnemyDetalisHidden, Vector3.zero);
    }

    private void OnCardRightClickBackFUNC(SnapBackTweenRequested msg)
    {
        if (msg.MovementName == "Click")
            FMODUnity.RuntimeManager.PlayOneShot(OnCardRightClickBack, Vector3.zero);
    }

    private void OnCardRightClickFUNC(TweenMovementRequested msg)
    {
        if (msg.MovementName == "Click")
            FMODUnity.RuntimeManager.PlayOneShot(OnCardRightClick, Vector3.zero);
    }

    private void OnCardReFUNC(CardResolutionStarted msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardReZone, Vector3.zero);
    }

    private void OnEnemyHoverFUNC(CharacterHoverChanged msg)
    {
        try
        {
            if (msg.HoverCharacter.IsPresentAnd(m => m.Member.TeamType == TeamType.Enemies))
                FMODUnity.RuntimeManager.PlayOneShot(OnTooltipHover, Vector3.zero);
        }
        catch (Exception e) { }
    }
    
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

    private void OnCardCancelled(PlayerCardCanceled msg)
    {
        FMODUnity.RuntimeManager.PlayOneShot(OnCardDiscarded, Vector3.zero);
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
