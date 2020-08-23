using System.Linq;

public class RepeatForSpentCondition : Condition
{
    private readonly CardActionsData _effect;

    public RepeatForSpentCondition(CardActionsData effect) => _effect = effect;

    public IPayloadProvider Resolve(BattleStateSnapshot beforeCard, Member source, Target target, Group @group, Scope scope, int amountPaid)
        => new MultiplePayloads(Enumerable.Range(0, amountPaid).Select(x => _effect.Play(source, target, @group, scope, 1)).ToArray());
}