public class ApplyBattleEffect
{
    public bool IsFirstBattleEffect { get; }
    public ReactionTimingWindow Timing => Effect.FinalReactionTimingWindow;
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
        : this(isFirstBattleEffect, effect, source, target, card, xPaidAmount, preventions, default(Group), default(Scope), true) {}
    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, PreventionContext preventions, bool isReaction)
        : this(isFirstBattleEffect, effect, source, target, card, xPaidAmount, preventions, default(Group), default(Scope), isReaction) {}
    public ApplyBattleEffect(bool isFirstBattleEffect, EffectData effect, Member source, Target target, Maybe<Card> card, 
        ResourceQuantity xPaidAmount, PreventionContext preventions, Group targetGroup, Scope scope, bool isReaction)
    {
        IsFirstBattleEffect = isFirstBattleEffect;
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = isFirstBattleEffect;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
        Card = card;
        XPaidAmount = xPaidAmount;
        Preventions = preventions;
    }
}
