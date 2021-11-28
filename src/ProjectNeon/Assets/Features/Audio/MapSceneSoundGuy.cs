
using FMOD.Studio;
using UnityEngine;

public class MapSceneSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpClicked;
    [SerializeField, FMODUnity.EventRef] private string OnTravel;
    [SerializeField, FMODUnity.EventRef] private string OnEQpurchased;
    [SerializeField, FMODUnity.EventRef] private string OnCardpurchased;
    [SerializeField, FMODUnity.EventRef] private string OnHeroLevelledUp;
    [SerializeField, FMODUnity.EventRef] private string OnDieRollShake;
    [SerializeField, FMODUnity.EventRef] private string OnDieThrown;
    [SerializeField, FMODUnity.EventRef] private string OnArriveAtCombat;
    [SerializeField, FMODUnity.EventRef] private string OnEliteCombat;
    [SerializeField, FMODUnity.EventRef] private string OnCardShop;
    [SerializeField, FMODUnity.EventRef] private string OnGearShop;
    [SerializeField, FMODUnity.EventRef] private string OnClinic;
    [SerializeField, FMODUnity.EventRef] private string OnBoss;
    [SerializeField, FMODUnity.EventRef] private string OnStoryEvent;
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpHover;
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpStat;
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpOptionRevealed;
    [SerializeField, FMODUnity.EventRef] private string OnLevelUpOptionSelectedWhoosh;

    private bool _debuggingLoggingEnabled = false;
    private EventInstance _levelUpStinger;

    private void OnEnable()
    {
        Message.Subscribe<LevelUpClicked>(e =>
        {
            PlayOneShot(OnLevelUpClicked, e.UiSource);
            PlayOneShot(OnLevelUpOptionSelectedWhoosh, e.UiSource);
        }, this);
        Message.Subscribe<HeroLeveledUpSFX>(e => PlayLevelUpStinger(), this);
        Message.Subscribe<StatLeveledUp>(e => PlayOneShot(OnLevelUpStat, e.UiSource), this);
        
        Message.Subscribe<StoryEventBegun>(e => PlayOneShot(OnStoryEvent, e.UiSource), this);
        Message.Subscribe<TravelingSFX>(e => PlayOneShot(OnTravel, e.UiSource), this);
        Message.Subscribe<EQpurchased>(e => PlayOneShot(OnEQpurchased, e.UiSource), this);
        Message.Subscribe<CardPurchased>(e => PlayOneShot(OnCardpurchased, e.UiSource), this);
        Message.Subscribe<DieRollShaking>(e => PlayOneShot(OnDieRollShake, e.UiSource), this);
        Message.Subscribe<DieThrown>(e => PlayOneShot(OnDieThrown, e.UiSource), this);
        Message.Subscribe<ArrivedAtNode>(OnArrivedAtNode, this);
        Message.Subscribe<PlayUiSound>(OnPlayUiSound, this);
    }

    private void OnPlayUiSound(PlayUiSound msg)
    {
        if (msg.Name.Equals("LevelUpOptionReveal"))
            PlayOneShot(OnLevelUpOptionRevealed, msg.UiSource);
    }

    private void OnArrivedAtNode(ArrivedAtNode node)
    {
        if (node.NodeType == MapNodeType.Combat)
            PlayOneShot(OnArriveAtCombat, node.UiSource);
        if (node.NodeType == MapNodeType.Elite)
            PlayOneShot(OnEliteCombat, node.UiSource);
        if (node.NodeType == MapNodeType.CardShop)
            PlayOneShot(OnCardShop, node.UiSource);
        if (node.NodeType == MapNodeType.GearShop)
            PlayOneShot(OnGearShop, node.UiSource);
        if (node.NodeType == MapNodeType.Clinic)
            PlayOneShot(OnClinic, node.UiSource);
        if (node.NodeType == MapNodeType.Boss)
            PlayOneShot(OnBoss, node.UiSource);
    }

    private void PlayLevelUpStinger()
    {
        if (!_levelUpStinger.isValid())
            _levelUpStinger = FMODUnity.RuntimeManager.CreateInstance(OnHeroLevelledUp);

        _levelUpStinger.stop(STOP_MODE.IMMEDIATE);
        _levelUpStinger.start();
    }
    
    private void OnDisable()
    {
        if (_levelUpStinger.isValid())
            _levelUpStinger.release();
        Message.Unsubscribe(this);
    }
    
    private void PlayOneShot(string eventName, Transform uiSource)
    {
        FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
    }

    private void DebugLog(string msg)
    {
        if (_debuggingLoggingEnabled)
            Log.Info(msg);
    }
}
