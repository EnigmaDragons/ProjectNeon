
public class EndOfTurnResourceGainPersistentState : IPersistentState
{
    private readonly ResourceQuantity _qty;
    private readonly Member _member;
    private readonly PartyAdventureState _partyState;

    public EndOfTurnResourceGainPersistentState(ResourceQuantity qty, Member member, PartyAdventureState partyState)
    {
        _qty = qty;
        _member = member;
        _partyState = partyState;
    }
    
    public void OnTurnStart() {}
    public void OnTurnEnd()
    {
        if (_qty.Amount == 0)
            return;
        
        BattleLog.Write($"{_member.Name} gained {_qty}");
        _member.State.Gain(_qty, _partyState);
    }
}
