public class PerformerHasResourceCondition : Condition
{
    private readonly int _amount;
    private readonly string _resource;
    private readonly CardActionsData _effect;

    public PerformerHasResourceCondition(int amount, string resource, CardActionsData effect)
    {
        _amount = amount;
        _resource = resource;
        _effect = effect;
    }

    public IPayloadProvider Resolve(Member source, Target target, Group @group, Scope scope, int amountPaid)
    {
        if (source.State[new InMemoryResourceType { Name = _resource }] >= _amount)
            return _effect.Play(source, target, group, scope, amountPaid);
        return new NoPayload();
    }
}