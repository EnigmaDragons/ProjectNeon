public class ApplyBattleEffect
{
    public EffectData Effect { get; }
    public Member Source { get; }
    public Target Target { get; }
    public bool CanRetarget { get; }
    public Group Group { get; } 
    public Scope Scope { get; }
    public bool IsReaction { get; }
    public Maybe<Card> Card { get; }
    public ResourceQuantity XPaidAmount { get; }
    

    public ApplyBattleEffect(EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount)
    {
        Effect = effect;
        Source = source;
        Target = target;
        IsReaction = true;
        Card = card;
        XPaidAmount = xPaidAmount;
    }
        
    public ApplyBattleEffect(EffectData effect, Member source, Target target, Maybe<Card> card, ResourceQuantity xPaidAmount, Group targetGroup, Scope scope, bool isReaction)
    {
        Effect = effect;
        Source = source;
        Target = target;
        CanRetarget = true;
        Group = targetGroup;
        Scope = scope;
        IsReaction = isReaction;
        Card = card;
        XPaidAmount = xPaidAmount;
    }
}
