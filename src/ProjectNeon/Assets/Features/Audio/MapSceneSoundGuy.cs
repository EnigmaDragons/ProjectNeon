
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

    private bool debuggingLoggingEnabled = false;

    private void OnEnable()
    {
        Message.Subscribe<ShowTooltip>(e => PlayOneShot(OnLevelUpClicked, e.UiSource), this);
        Message.Subscribe<LevelUpClicked>(e => PlayOneShot(OnLevelUpClicked, e.UiSource), this);
        Message.Subscribe<TravelingSFX>(e => PlayOneShot(OnTravel, e.UiSource), this);
        Message.Subscribe<EQpurchased>(e => PlayOneShot(OnEQpurchased, e.UiSource), this);
        Message.Subscribe<CardPurchased>(e => PlayOneShot(OnCardpurchased, e.UiSource), this);
        Message.Subscribe<HeroLeveledUpSFX>(e => PlayOneShot(OnHeroLevelledUp, e.UiSource), this);
        Message.Subscribe<DieRollShaking>(e => PlayOneShot(OnDieRollShake, e.UiSource), this);
        Message.Subscribe<DieThrown>(e => PlayOneShot(OnDieThrown, e.UiSource), this);
        Message.Subscribe<ArrivedAtNode>(OnArrivedAtNode, this);
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
        if (node.NodeType == MapNodeType.StoryEvent)
            PlayOneShot(OnStoryEvent, node.UiSource);

    }
    
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
