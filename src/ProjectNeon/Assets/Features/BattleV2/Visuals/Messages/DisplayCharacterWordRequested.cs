
public class DisplayCharacterWordRequested
{
    public int MemberId { get; }
    public CharacterReactionType ReactionType { get; }

    public DisplayCharacterWordRequested(Member m, CharacterReactionType reactionType)
        : this(m.Id, reactionType) {}
    
    public DisplayCharacterWordRequested(int memberId, CharacterReactionType reactionType)
    {
        MemberId = memberId;
        ReactionType = reactionType;
    }
}
