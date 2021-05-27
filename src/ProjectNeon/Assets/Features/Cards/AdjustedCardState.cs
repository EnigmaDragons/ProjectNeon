public class AdjustedCardState : ITemporalCardState
{
    private int _turnsRemaining;
    private int _cardPlaysRemaining;
    
    public int CostAdjustment { get; }

    public bool IsActive => _turnsRemaining != 0 && _cardPlaysRemaining != 0;

    public AdjustedCardState(int turnsRemaining, int cardPlaysRemaining, int costAdjustment)
    {
        _turnsRemaining = turnsRemaining;
        _cardPlaysRemaining = cardPlaysRemaining;
        CostAdjustment = costAdjustment;
    }
    
    public void OnCardPlay()
    {
        if (_cardPlaysRemaining > 0)
            _cardPlaysRemaining--;
    }

    public void OnTurnStart() {}

    public void OnTurnEnd()
    {
        if (_turnsRemaining > 0)
            _turnsRemaining--;
    }
}