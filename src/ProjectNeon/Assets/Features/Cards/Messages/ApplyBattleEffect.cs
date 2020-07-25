public class ApplyBattleEffect
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }

    public ApplyBattleEffect(EffectData effect, Member source, Target target)
    {
        Effect = effect;
        Source = source;
        Target = target;
    }
}