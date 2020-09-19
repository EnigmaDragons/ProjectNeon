public sealed class CharacterAnimationRequested
{
    public int MemberId { get; }
    public AnimationData Animation { get; }

    public CharacterAnimationRequested(int memberId, AnimationData animation)
    {
        MemberId = memberId;
        Animation = animation;
    }
}
