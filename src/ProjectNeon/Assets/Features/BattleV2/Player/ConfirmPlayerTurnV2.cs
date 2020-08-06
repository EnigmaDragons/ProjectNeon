using UnityEngine;

public class ConfirmPlayerTurnV2 : MonoBehaviour, IConfirmCancellable
{
    [SerializeField] private CardPlayZone playArea;
    [SerializeField] private GameObject confirmUi;

    private bool _isConfirming = false;
    private bool _confirmRequested;
    
    public bool CanConfirm => playArea.Cards.Length == 3 || _confirmRequested;

    private void OnEnable()
    {
        Message.Subscribe<BeginPlayerTurnConfirmation>(_ => OnConfirmationRequested(), this);
        playArea.OnZoneCardsChanged.Subscribe(UpdateState, this);
    }

    private void OnDisable()
    {
        Message.Unsubscribe(this);
        playArea.OnZoneCardsChanged.Unsubscribe(this);
    }

    private void OnConfirmationRequested()
    {
        _confirmRequested = true;
        UpdateState();
    }
    
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

        _confirmRequested = false;
        playArea.Clear();
        BattleLog.Write("Player Confirmed Turn");
        Message.Publish(new PlayerTurnConfirmed());
    }

    public void Cancel()
    {
        _confirmRequested = false;
        UpdateState();
    }
}
