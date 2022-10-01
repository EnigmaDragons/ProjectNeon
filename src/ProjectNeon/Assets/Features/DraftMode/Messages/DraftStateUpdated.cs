
public class DraftStateUpdated
{
    public int StepNumber { get; }
    public int TotalStepsCount { get; }
    public int HeroIndex { get; }

    public DraftStateUpdated(int stepNumber, int totalStepsCount, int heroIndex)
    {
        StepNumber = stepNumber;
        TotalStepsCount = totalStepsCount;
        HeroIndex = heroIndex;
    }
}
