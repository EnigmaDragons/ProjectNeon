public class ApplyBattleEffect
{
    public bool IsFirstBattleEffect { get; }
    public ReactionTimingWindow Timing { get; }
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

    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, 
        PreventionContext preventions, ReactionTimingWindow timing)
        : this(isFirstBattleEffect, effect, source, target, card, xPaidAmount, preventions, default(Group), default(Scope), true, timing) {}
    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, 
        PreventionContext preventions, bool isReaction, ReactionTimingWindow timing)
        : this(isFirstBattleEffect, effect, source, target, card, xPaidAmount, preventions, default(Group), default(Scope), isReaction, timing) {}
    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, 
        ResourceQuantity xPaidAmount, PreventionContext preventions, Group targetGroup, Scope scope, bool isReaction, ReactionTimingWindow timing)
    {
        IsFirstBattleEffect = isFirstBattleEffect;
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = isFirstBattleEffect;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
        Timing = timing;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
    }
}
