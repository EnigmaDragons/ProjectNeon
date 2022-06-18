
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

    public IPayloadProvider OnTurnStart() => new NoPayload();
    public IPayloadProvider OnTurnEnd() => new SinglePayload(new PerformAction(() => _member.State.ChangeResource(_qty, _partyState)));
}
