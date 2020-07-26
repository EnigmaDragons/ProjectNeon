using UnityEngine;

public class ConfirmPlayerTurnV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private GameObject confirmUi;

    private bool _isConfirming = false;
    
    public bool CanConfirm => playArea.Cards.Length == 3;

    private void OnEnable() => playArea.OnZoneCardsChanged.Subscribe(UpdateState, this);
    private void OnDisable() => playArea.OnZoneCardsChanged.Unsubscribe(this);

    private void UpdateState()
    {
        if (_isConfirming == CanConfirm)
            return;
        
        _isConfirming = CanConfirm;
        confirmUi.SetActive(_isConfirming);
        if (_isConfirming)
            Message.Publish(new PlayerTurnConfirmationStarted());
        else
            Message.Publish(new PlayerTurnConfirmationAborted());
    }

    public void Confirm()
    {
        if (!CanConfirm) return;
    
        playArea.Clear();
        BattleLog.Write("Player Confirmed Turn");
        Message.Publish(new PlayerTurnConfirmed());
    }

    public void Cancel() {}
}
