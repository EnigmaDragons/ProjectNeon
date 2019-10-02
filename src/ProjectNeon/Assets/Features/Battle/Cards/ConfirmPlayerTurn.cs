using UnityEngine;

public class ConfirmPlayerTurn : MonoBehaviour
{
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private GameEvent onConfirmed;
    [SerializeField] private GameEvent onConfirmationStarted;
    [SerializeField] private GameObject confirmUi;

    private bool CanConfirm => playArea.Cards.Length == 3;

    private void OnEnable()
    {
        playArea.OnZoneCardsChanged.Subscribe(UpdateState, this);
    }

    private void OnDisable()
    {
        playArea.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void UpdateState()
    {
        confirmUi.SetActive(CanConfirm);
        if (CanConfirm)
            onConfirmationStarted.Publish();
    }

    public void Confirm()
    {
        if (CanConfirm)
            onConfirmed.Publish();
    }
}
