using System.Linq;

public class GainStateIfInZoneAtEndOfTurn : ITemporalCardState
{
    private ITemporalCardState _stateToGain;
    private Card _card;
    private CardPlayZone _zone;
    
    public bool IsActive { get; set; } = true;
    public int CostAdjustment => 0;

    public GainStateIfInZoneAtEndOfTurn(ITemporalCardState stateToGain, Card card, CardPlayZone zone)
    {
        _stateToGain = stateToGain;
        _card = card;
        _zone = zone;
    }

    public void OnCardPlay() {}

    public void OnTurnStart() {}

    public void OnTurnEnd()
    {
        if (_zone.Cards.Any(x => x == _card))
            _card.AddState(_stateToGain);
        IsActive = false;
    }
}