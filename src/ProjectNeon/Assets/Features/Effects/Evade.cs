
public class Evade : Effect
{
    private int _number;

    public Evade(int number) => _number = number;

    public void Apply(Member source, Target target)
    {
        target.ApplyToAllConscious(x => x.AdjustEvade(_number));
    }
}