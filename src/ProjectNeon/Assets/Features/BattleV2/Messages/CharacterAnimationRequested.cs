public sealed class CharacterAnimationRequested
{
    public int MemberId { get; }
    public string Animation { get; }

    public CharacterAnimationRequested(int memberId, string animation)
    {
        MemberId = memberId;
        Animation = animation;
    }
}
