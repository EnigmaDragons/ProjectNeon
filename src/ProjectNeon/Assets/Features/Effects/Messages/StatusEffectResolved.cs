
public class StatusEffectResolved
{
    public Member Member { get; }
    public EffectResolved EffectResolved { get; }

    public StatusEffectResolved(Member m, EffectResolved e)
    {
        Member = m;
        EffectResolved = e;
    }
}
