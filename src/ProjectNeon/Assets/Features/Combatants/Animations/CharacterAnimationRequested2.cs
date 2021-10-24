public class CharacterAnimationRequested2
{
    public int MemberId { get; }
    public CharacterAnimationType Animation { get; }

    public CharacterAnimationRequested2(int memberId, CharacterAnimationType animation)
    {
        MemberId = memberId;
        Animation = animation;
    }
}