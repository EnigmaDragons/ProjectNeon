
public sealed class RecurrentEffect :  Effect
{
    private int _repetitions;
    private int _count;
    private Effect _effect;

    public RecurrentEffect(Effect origin, int times)
    {
        _effect = origin;
        _repetitions = times;
        _count = 0;

    }

    public void Apply(Member source, Target target)
    {
        if (_count < _repetitions)
        {
            _effect.Apply(source, target);
            _count++;
        }
    }
}
