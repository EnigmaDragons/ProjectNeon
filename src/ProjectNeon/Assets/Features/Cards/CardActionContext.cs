using System.Collections.Generic;

public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; } 
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity AmountPaid { get; }
    public BattleStateSnapshot BeforeState { get; }

    public CardActionContext(Member source, Target target, Group group, Scope scope, ResourceQuantity amountPaid, BattleStateSnapshot beforeState)
    {
        Source = source;
        Target = target;
        Group = @group;
        Scope = scope;
        AmountPaid = amountPaid;
        BeforeState = beforeState;
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, Group, Scope, AmountPaid, BeforeState);
}
