public interface Condition
{
    IPayloadProvider Resolve(BattleStateSnapshot beforeCard, Member source, Target target, Group group, Scope scope, int amountPaid);
}