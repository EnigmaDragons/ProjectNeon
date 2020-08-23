using System.Linq;

public class TargetSufferedDamageCondition : Condition
{
    private readonly CardActionsData _effect;

    public TargetSufferedDamageCondition(CardActionsData effect) => _effect = effect;
    
    public IPayloadProvider Resolve(BattleStateSnapshot beforeCard, Member source, Target target, Group @group, Scope scope, int amountPaid)
    {
        var applicapleMembers = target.Members.Where(x => beforeCard.Members[x.Id].State.Counters["HP"] > x.State[TemporalStatType.HP]).ToArray();
        if (applicapleMembers.Any())
            return _effect.Play(source, new Multiple(applicapleMembers), group, scope, amountPaid);
        return new NoPayload();
    }
}