
public class DisplayCharacterWordRequested
{
    public Member Member { get; }
    public string Word { get; }

    public DisplayCharacterWordRequested(Member m, string word)
    {
        Member = m;
        Word = word;
    }
}
