public class CardActionAvoided
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public Member[] AvoidingMembers { get; }

    public CardActionAvoided(EffectData effect, Member source, Target target, Member[] avoidingMembers)
    {
        Effect = effect;
        Source = source;
        Target = target;
        AvoidingMembers = avoidingMembers;
    }
}