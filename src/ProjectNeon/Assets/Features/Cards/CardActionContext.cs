
public sealed class CardActionContext
{
    public Member Source { get; } 
    public Target Target { get; }
    public Member[] AvoidingMembers { get; } 
    public Group Group { get; } 
    public Scope Scope { get; }
    public ResourceQuantity XPaidAmount { get; }
    public BattleStateSnapshot BeforeState { get; }

    public CardActionContext(Member source, Target target, Member[] avoidingMembers, Group group, Scope scope, ResourceQuantity xPaidAmount, BattleStateSnapshot beforeState)
    {
        Source = source;
        Target = target;
        AvoidingMembers = avoidingMembers;
        Group = @group;
        Scope = scope;
        XPaidAmount = xPaidAmount;
        BeforeState = beforeState;
    }
    
    public CardActionContext WithTarget(Target target)
        => new CardActionContext(Source, target, AvoidingMembers, Group, Scope, XPaidAmount, BeforeState);
}
