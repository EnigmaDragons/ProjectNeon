using UnityEngine;

public class DeckBuilderSoundGuy : MonoBehaviour
{
    [SerializeField, FMODUnity.EventRef] private string OnEquipmentClicked;
    [SerializeField, FMODUnity.EventRef] private string OnCardAddedToDeck;

    private void OnEnable()
    {
        Message.Subscribe<EquipmentPickerCurrentGearChanged>(e => PlayOneShot(OnEquipmentClicked, e.UiSource), this);
        Message.Subscribe<CardAddedToDeck>(e => PlayOneShot(OnCardAddedToDeck, e.UiSource), this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);   
    }

    private void PlayOneShot(string eventName, Transform uiSource) 
        => FMODUnity.RuntimeManager.PlayOneShot(OnEquipmentClicked, uiSource.position);
}
