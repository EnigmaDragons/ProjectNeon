using UnityEngine;

public class DeckBuilderSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentClicked;
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentEquipped;
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentUnequipped;
    [SerializeField, FMODUnity.EventRef] private string OnCardAddedToDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardRemovedFromDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardHovered;
    [SerializeField, FMODUnity.EventRef] private string OnCardHoveredOnDeck;
    [SerializeField, FMODUnity.EventRef] private string OnArchetypeToggled;
    [SerializeField, FMODUnity.EventRef] private string OnBattleStart;

    private void OnEnable()
    {
        Message.Subscribe<EquipmentPickerCurrentGearChanged>(e => OnEquipped(e), this);
        Message.Subscribe<CardAddedToDeck>(e => PlayOneShot(OnCardAddedToDeck, e.UiSource), this);
        Message.Subscribe<CardRemovedFromDeck>(e => PlayOneShot(OnCardRemovedFromDeck, e.UiSource), this);
       //Message.Subscribe<CardHovered>(e => PlayOneShot(OnCardHovered, e.UiSource), this);
       Message.Subscribe<CardHoveredOnDeck>(e => PlayOneShot(OnCardHoveredOnDeck, e.UiSource), this);
        //Message.Subscribe<ArchetypeToggled>(e => PlayOneShot(OnArchetypeToggled, e.UiSource), this);
        Message.Subscribe<StartBattleInitiated>(e => PlayBattleStart (e), this);
    }

    private void OnEquipped(EquipmentPickerCurrentGearChanged msg)
    {
        if (msg.IsEquipped)
            PlayOneShot(OnEquipmentEquipped, msg.UiSource);
        else if (!msg.IsEquipped)
            PlayOneShot(OnEquipmentUnequipped, msg.UiSource);
    }
    private void PlayBattleStart(StartBattleInitiated msg)
    {
        Log.Info("BattleStart");
            PlayOneShot(OnBattleStart, msg.UiSource);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);   
    }

    private void PlayOneShot(string eventName, Transform uiSource) 
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
}
