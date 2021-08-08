public class ApplyBattleEffect
{
    public bool IsFirstBattleEffect { get; }
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public bool CanRetarget { get; }
    public Group Group { get; } 
    public Scope Scope { get; }
    public bool IsReaction { get; }
    public Maybe<Card> Card { get; }
    public ResourceQuantity XPaidAmount { get; }
    public PreventionContext Preventions { get; }

    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PreventionContext preventions)
    {
        IsFirstBattleEffect = isFirstBattleEffect;
        Effect = effect;
        Source = source;
        Target = target;
        IsReaction = true;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
    }
        
    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PreventionContext preventions, Group targetGroup, Scope scope, bool isReaction)
    {
        IsFirstBattleEffect = isFirstBattleEffect;
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = true;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
    }
}
