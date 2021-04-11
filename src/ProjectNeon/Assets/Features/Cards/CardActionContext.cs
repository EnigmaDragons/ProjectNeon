
public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; }
    public PreventionContext Preventions { get; }
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XAmountPaid { get; }
    public BattleStateSnapshot BeforeState { get; }
    public Maybe<Card> Card { get; }

    public CardActionContext(Member source, Target target, Group group, Scope scope, ResourceQuantity xAmountPaid, BattleStateSnapshot beforeState, Maybe<Card> card)
    {
        Source = source;
        Target = target;
        Group = @group;
        Scope = scope;
        XAmountPaid = xAmountPaid;
        BeforeState = beforeState;
        Card = card;
        Preventions = new PreventionContextMut(target);
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, Group, Scope, XAmountPaid, BeforeState, Card);
}
