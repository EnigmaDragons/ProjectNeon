using UnityEngine;

public class DeckBuilderSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentClicked;
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentEquipped;
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentUnequipped;
    [SerializeField, FMODUnity.EventRef] private string OnCardAddedToDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardRemovedFromDeck;
    [SerializeField, FMODUnity.EventRef] private string OnCardHovered;

    private void OnEnable()
    {
        Message.Subscribe<EquipmentPickerCurrentGearChanged>(e => OnEquipped(e), this);
        Message.Subscribe<CardAddedToDeck>(e => PlayOneShot(OnCardAddedToDeck, e.UiSource), this);
        Message.Subscribe<CardRemovedFromDeck>(e => PlayOneShot(OnCardRemovedFromDeck, e.UiSource), this);
        Message.Subscribe<CardHovered>(e => PlayOneShot(OnCardHovered, e.UiSource), this);
    }

    private void OnEquipped(EquipmentPickerCurrentGearChanged msg)
    {
        if (msg.IsEquipped)
            PlayOneShot(OnEquipmentEquipped, msg.UiSource);
        else if (!msg.IsEquipped)
            PlayOneShot(OnEquipmentUnequipped, msg.UiSource);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);   
    }

    private void PlayOneShot(string eventName, Transform uiSource) 
        => FMODUnity.RuntimeManager.PlayOneShot(eventName, uiSource.position);
}
