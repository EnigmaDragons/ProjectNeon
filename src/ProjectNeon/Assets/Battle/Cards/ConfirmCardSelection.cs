using UnityEngine;

public class ConfirmCardSelection : MonoBehaviour
{
    [SerializeField] private CardPlayZones playZones;
    [SerializeField] private GameObject confirmUi;
    public GameEvent onConfirmation;

    private bool CanConfirm => playZones.PlayZone.Cards.Length == 3;

    private void OnEnable()
    {
        playZones.PlayZone.OnZoneCardsChanged.Subscribe(UpdateUiElement, this);
    }

    private void OnDisable()
    {
        playZones.PlayZone.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void UpdateUiElement()
    {
        confirmUi.SetActive(CanConfirm);
    }

    // @todo #133: 50min Add resolution zone
    public void Confirm()
    {
        if (CanConfirm)
        {
            onConfirmation.Publish();
            confirmUi.SetActive(false);
        }
            
    }
}
