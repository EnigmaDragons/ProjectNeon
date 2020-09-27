using System.Collections.Generic;

public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; } 
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XPaidAmount { get; }
    public BattleStateSnapshot BeforeState { get; }

    public CardActionContext(Member source, Target target, Group group, Scope scope, ResourceQuantity xPaidAmount, BattleStateSnapshot beforeState)
    {
        Source = source;
        Target = target;
        Group = @group;
        Scope = scope;
        XPaidAmount = xPaidAmount;
        BeforeState = beforeState;
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, Group, Scope, XPaidAmount, BeforeState);
}
