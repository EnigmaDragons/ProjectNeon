
public class DisplayCharacterWordRequested
{
    public int MemberId { get; }
    public string Word { get; }

    public DisplayCharacterWordRequested(Member m, string word)
        : this(m.Id, word) {}
    
    public DisplayCharacterWordRequested(int memberId, string word)
    {
        MemberId = memberId;
        Word = word;
    }
}
