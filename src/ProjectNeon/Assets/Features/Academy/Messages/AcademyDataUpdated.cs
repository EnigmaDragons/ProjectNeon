public class AcademyDataUpdated
{
    public AcademyDataSnapshot Before { get; }
    public AcademyDataSnapshot After { get; }

    public AcademyDataUpdated(AcademyDataSnapshot before, AcademyDataSnapshot after)
    {
        Before = before;
        After = after;
    }
}
