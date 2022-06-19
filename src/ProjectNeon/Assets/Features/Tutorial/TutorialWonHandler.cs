
public class TutorialWonHandler : OnMessage<TutorialWon>
{
    protected override void Execute(TutorialWon msg) => Execute();

    private const int TutorialAdventureId = 10;
    
    public static void Execute(float delay = 3f)
    {
        Log.Info("Finished Tutorial Adventure");
        CurrentAcademyData.Write(a =>
        {
            a.TutorialData = new AcademyTutorialData { CompletedTutorialNames = AcademyData.RequiredLicenseTutorials.ToArray() };
            return a;
        });
        CurrentProgressionData.RecordCompletedAdventure(TutorialAdventureId);
        CurrentGameData.Clear();
        Message.Publish(new ExecuteAfterDelayRequested(delay, () => Message.Publish(new NavigateToNextTutorialFlow())));
    }
}
