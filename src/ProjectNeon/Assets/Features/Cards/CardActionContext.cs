
public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; }
    public Member[] AvoidingMembers { get; } 
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XAmountPaid { get; }
    public BattleStateSnapshot BeforeState { get; }
    public Maybe<Card> Card { get; }

    public CardActionContext(Member source, Target target, Member[] avoidingMembers, Group group, Scope scope, ResourceQuantity xAmountPaid, BattleStateSnapshot beforeState, Maybe<Card> card)
    {
        Source = source;
        Target = target;
        AvoidingMembers = avoidingMembers;
        Group = @group;
        Scope = scope;
        XAmountPaid = xAmountPaid;
        BeforeState = beforeState;
        Card = card;
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, AvoidingMembers, Group, Scope, XAmountPaid, BeforeState, Card);
}
