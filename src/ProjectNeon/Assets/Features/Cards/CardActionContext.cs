
public sealed class CardActionContext
{
    private readonly AvoidanceContext _avoidance;
    
    public Member Source { get; } 
    public Target Target { get; }
    public AvoidanceContext Avoid => _avoidance;
    public PreventionContext Preventions { get; }
    //public AvoidanceType AvoidanceType => _avoidance.Type;
    //public Member[] AvoidingMembers => _avoidance.AvoidingMembers;
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XAmountPaid { get; }
    public BattleStateSnapshot BeforeState { get; }
    public Maybe<Card> Card { get; }

    public CardActionContext(Member source, Target target, AvoidanceContext avoid, Group group, Scope scope, ResourceQuantity xAmountPaid, BattleStateSnapshot beforeState, Maybe<Card> card)
    {
        Source = source;
        Target = target;
        _avoidance = avoid;
        Group = @group;
        Scope = scope;
        XAmountPaid = xAmountPaid;
        BeforeState = beforeState;
        Card = card;
        Preventions = new PreventionContextMut(target);
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, _avoidance, Group, Scope, XAmountPaid, BeforeState, Card);
}
