public sealed class CharacterAnimationRequested
{
    public int MemberId { get; }
    public IAnimationData Animation { get; }
    public Target Target { get; }
    
    public CharacterAnimationRequested(int memberId, IAnimationData animation, Target target)
    {
        MemberId = memberId;
        Animation = animation;
        Target = target;
    }
}
