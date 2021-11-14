using UnityEngine;

public class DeckBuilderSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentEquipped;
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentUnequipped;
    [SerializeField, FMODUnity.EventRef] private string OnCardAddedToDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardRemovedFromDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardHoveredOnDeck;
    [SerializeField, FMODUnity.EventRef] private string OnArchetypeToggled;
    [SerializeField, FMODUnity.EventRef] private string OnBattleStart;
    [SerializeField, FMODUnity.EventRef] private string Error;

    private bool debuggingLoggingEnabled = false;

    private void OnEnable()
    {
        Message.Subscribe<EquipmentPickerCurrentGearChanged>(e => OnEquipped(e), this);
        Message.Subscribe<CardAddedToDeck>(e => PlayOneShot(OnCardAddedToDeck, e.UiSource), this);
        Message.Subscribe<CardRemovedFromDeck>(e => PlayOneShot(OnCardRemovedFromDeck, e.UiSource), this);
        Message.Subscribe<CardHoveredOnDeck>(e => PlayOneShot(OnCardHoveredOnDeck, e.UiSource), this);
        Message.Subscribe<ArchetypeToggled>(e => ArchToggled(e), this);
        Message.Subscribe<StartBattleInitiated>(e => PlayBattleStart(e), this);
        Message.Subscribe<CardAddToDeckAttemptRejected>(e => PlayOneShot(Error, e.UiSource), this);
    }

    private void ArchToggled(ArchetypeToggled msg)
    {
        DebugLog("Sound - ArchToggled");
        PlayOneShot(OnArchetypeToggled, msg.UiSource);
    }

    private void OnEquipped(EquipmentPickerCurrentGearChanged msg)
    {
        var actionStr = msg.IsEquipped ? "equipped" : "unequipped";
        DebugLog($"Sound - Equipment {actionStr}");
        if (msg.IsEquipped)
            PlayOneShot(OnEquipmentEquipped, msg.UiSource);
        else if (!msg.IsEquipped)
            PlayOneShot(OnEquipmentUnequipped, msg.UiSource);
    }
    
    private void PlayBattleStart(StartBattleInitiated msg)
    {
        DebugLog("Sound - BattleStart");
        PlayOneShot(OnBattleStart, msg.UiSource);
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
