public class ApplyBattleEffect
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public bool CanRetarget { get; }
    public Group Group { get; } 
    public Scope Scope { get; }
    public bool IsReaction { get; }

    public ApplyBattleEffect(EffectData effect, Member source, Target target)
    {
        Effect = effect;
        Source = source;
        Target = target;
        IsReaction = true;
    }
        
    public ApplyBattleEffect(EffectData effect, Member source, Target target, Group targetGroup, Scope scope, bool isReaction)
    {
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = true;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
    }
}
