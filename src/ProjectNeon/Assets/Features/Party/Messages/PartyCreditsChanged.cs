
public class PartyCreditsChanged 
{
    public int Before { get; }
    public int After { get; }
    public int Delta => After - Before;

    public PartyCreditsChanged(int before, int after)
    {
        Before = before;
        After = after;
    }
}