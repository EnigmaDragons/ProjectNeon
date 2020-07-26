using System.Linq;

public class TargetIsBelowPercentageLifeCondition : Condition
{
    private float _percentage;
    private CardActionsData _effect;

    public TargetIsBelowPercentageLifeCondition(float percentage, CardActionsData effect)
    {
        _percentage = percentage;
        _effect = effect;
    }
    
    public IPayloadProvider Resolve(Member source, Target target, Group @group, Scope scope, int amountPaid)
    {
        var applicapleMembers = target.Members.Where(x => x.State[TemporalStatType.HP] < x.State.MaxHP() * _percentage).ToArray();
        if (applicapleMembers.Any())
            return _effect.Play(source, new Multiple(applicapleMembers), group, scope, amountPaid);
        return new NoPayload();
    }
}