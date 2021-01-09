
public class EndOfTurnResourceGainPersistentState : IPersistentState
{
    private readonly ResourceQuantity _qty;
    private readonly Member _member;

    public EndOfTurnResourceGainPersistentState(ResourceQuantity qty, Member member)
    {
        _qty = qty;
        _member = member;
    }
    
    public void OnTurnStart() {}
    public void OnTurnEnd()
    {
        BattleLog.Write($"{_member.Name} gained {_qty}");
        _member.State.Gain(_qty);
    }
}
