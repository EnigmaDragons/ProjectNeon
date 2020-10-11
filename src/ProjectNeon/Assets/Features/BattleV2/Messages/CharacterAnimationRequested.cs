public sealed class CharacterAnimationRequested
{
    public int MemberId { get; }
    public AnimationData Animation { get; }
    public Target Target { get; }
    
    public CharacterAnimationRequested(int memberId, AnimationData animation, Target target)
    {
        MemberId = memberId;
        Animation = animation;
        Target = target;
    }
}
