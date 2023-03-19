public class CharacterAnimationRequested2
{
    public int MemberId { get; }
    public CharacterAnimationType Animation { get; }
    
    public Maybe<EffectCondition> Condition { get; set; } = Maybe<EffectCondition>.Missing();
    public Member Source { get; set; }
    public Target Target { get; set; }
    public Maybe<Card> Card { get; set; } = Maybe<Card>.Missing();
    public ResourceQuantity XPaidAmount { get; set; } = ResourceQuantity.None;
    public ResourceQuantity PaidAmount { get; set; } = ResourceQuantity.None;

    public CharacterAnimationRequested2(int memberId, CharacterAnimationType animation)
    {
        MemberId = memberId;
        Animation = animation;
    }
}
