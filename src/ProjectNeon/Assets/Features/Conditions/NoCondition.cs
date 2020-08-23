public class NoCondition : Condition
{
    public IPayloadProvider Resolve(BattleStateSnapshot beforeCard, Member source, Target target, Group @group, Scope scope, int amountPaid)
        => new MultiplePayloads(new IPayloadProvider[0]);
}