public class ApplyBattleEffect
{
    public bool IsFirstBattleEffectOfChosenTarget { get; }
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
    public ResourceQuantity PaidAmount { get; }
    public PreventionContext Preventions { get; }
    public DoubleDamageContext DoubleDamage { get; }
    
    public ApplyBattleEffect(bool isFirstBattleEffectOfChosenTarget, EffectData effect, Member source, Target target, Maybe<Card> card, 
        ResourceQuantity xPaidAmount, ResourceQuantity paidAmount, PreventionContext preventions, bool isReaction, ReactionTimingWindow timing, DoubleDamageContext doubleDamage)
        : this(isFirstBattleEffectOfChosenTarget, effect, source, target, card, xPaidAmount, paidAmount, preventions, default(Group), default(Scope), isReaction, timing, doubleDamage) {}
    public ApplyBattleEffect(bool isFirstBattleEffectOfChosenTarget, EffectData effect, Member source, Target target, Maybe<Card> card, 
        ResourceQuantity paidAmount, ResourceQuantity xPaidAmount, PreventionContext preventions, Group targetGroup, Scope scope, bool isReaction, 
        ReactionTimingWindow timing, DoubleDamageContext doubleDamage)
    {
        IsFirstBattleEffectOfChosenTarget = isFirstBattleEffectOfChosenTarget;
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = isFirstBattleEffectOfChosenTarget;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
        Timing = timing;
        Card = card;
        XPaidAmount = xPaidAmount;
        PaidAmount = paidAmount;
        Preventions = preventions;
        DoubleDamage = doubleDamage;
    }
}
