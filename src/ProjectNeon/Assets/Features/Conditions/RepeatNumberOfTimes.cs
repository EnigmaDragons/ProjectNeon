using System.Linq;

public class RepeatNumberOfTimes : ILogicFlow
{
    private readonly int _numberOfTimes;
    private readonly CardActionsData _effect;

    public RepeatNumberOfTimes(int numberOfTimes, CardActionsData effect)
    {
        _numberOfTimes = numberOfTimes;
        _effect = effect;
    }

    public IPayloadProvider Resolve(CardActionContext ctx)
        => new MultiplePayloads(Enumerable.Range(0, _numberOfTimes).Select(x => _effect.Play(ctx)));
}