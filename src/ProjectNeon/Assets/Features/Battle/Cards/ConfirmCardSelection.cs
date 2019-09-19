using UnityEngine;

public class ConfirmCardSelection : MonoBehaviour
{
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private GameEvent onConfirmation;
    [SerializeField] private GameObject confirmUi;

    private bool CanConfirm => playArea.Cards.Length == 3;

    private void OnEnable()
    {
        playArea.OnZoneCardsChanged.Subscribe(UpdateUiElement, this);
    }

    private void OnDisable()
    {
        playArea.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void UpdateUiElement()
    {
        confirmUi.SetActive(CanConfirm);
    }

    public void Confirm()
    {
        if (CanConfirm)
            onConfirmation.Publish();
    }
}
