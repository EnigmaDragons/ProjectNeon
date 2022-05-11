
public class PartyClinicVouchersChanged
{
    public int Before { get; }
    public int After { get; }
    public int Delta => After - Before;

    public PartyClinicVouchersChanged(int before, int after)
    {
        Before = before;
        After = after;
    }
}
