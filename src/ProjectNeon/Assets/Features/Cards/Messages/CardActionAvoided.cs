public class CardActionAvoided
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public AvoidanceContext Avoid { get; }

    public CardActionAvoided(EffectData effect, Member source, Target target, AvoidanceContext avoid)
    {
        Effect = effect;
        Source = source;
        Target = target;
        Avoid = avoid;
    }
}
