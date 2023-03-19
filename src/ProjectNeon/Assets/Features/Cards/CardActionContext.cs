
public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; }
    public PreventionContext Preventions { get; }
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XAmountPaid { get; }
    public ResourceQuantity AmountPaid { get; }
    public BattleStateSnapshot BeforeState { get; }
    public Maybe<Card> Card { get; }
    public DoubleDamageContext DoubleDamage { get; }

    public CardActionContext(Member source, Target target, Group group, Scope scope, ResourceQuantity xAmountPaid, ResourceQuantity amountPaid, BattleStateSnapshot beforeState, Maybe<Card> card, DoubleDamageContext doubleDamage)
    {
        Source = source;
        Target = target;
        Group = @group;
        Scope = scope;
        XAmountPaid = xAmountPaid;
        AmountPaid = amountPaid;
        BeforeState = beforeState;
        Card = card;
        Preventions = new PreventionContextMut(target);
        DoubleDamage = doubleDamage;
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, Group, Scope, XAmountPaid, AmountPaid, BeforeState, Card, DoubleDamage);
}
