using UnityEngine;

public class ConfirmPlayerTurnV2 : MonoBehaviour
{
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private GameObject confirmUi;

    private bool CanConfirm => playArea.Cards.Length == 3;

    private void OnEnable() => playArea.OnZoneCardsChanged.Subscribe(UpdateState, this);

    private void OnDisable() => playArea.OnZoneCardsChanged.Unsubscribe(this);

    private void UpdateState()
    {
        confirmUi.SetActive(CanConfirm);
        if (CanConfirm)
            Message.Publish(new PlayerConfirmationStarted());
    }

    public void Confirm()
    {
        if (!CanConfirm) return;
    
        playArea.Clear();
            Message.Publish(new PlayerTurnConfirmed());
    }
}
