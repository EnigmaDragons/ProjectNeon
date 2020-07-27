
public class Evade : Effect
{
    private Target _effectTarget;

    public void Apply(Member source, Target target)
    {
        _effectTarget = target;
        //implement
    }

    void Execute(Attack attack)
    {
        _effectTarget.Members.ForEach(
            target => {
                if (target.Equals(attack.Target.Members[0]))
                {
                    attack.Effect = new NoEffect();
                }
            }
        );
    }
}