public class CharacterAnimationRequested2
{
    public int MemberId { get; }
    public CharacterAnimationType Animation { get; }
    
    public Maybe<EffectCondition> Condition { get; set; }
    public Member Source { get; set; }
    public Target Target { get; set; }
    public Maybe<Card> Card { get; set; }
    public ResourceQuantity XPaidAmount { get; set; }

    public CharacterAnimationRequested2(int memberId, CharacterAnimationType animation)
    {
        MemberId = memberId;
        Animation = animation;
    }
}
