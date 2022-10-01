
using System.Linq;

public class TutorialWonHandler : OnMessage<TutorialWon>
{
    protected override void Execute(TutorialWon msg) => Execute();

    private const int TutorialAdventureId = 10;
    private const int TutorialDifficultyLevel = 0;
    private const int TutorialHeroId = 16;
    
    public static void Execute(float delay = 3f)
    {
        Log.Info("Finished Tutorial Adventure");        
        AllMetrics.PublishGameWon(TutorialAdventureId);
        CurrentAcademyData.Mutate(a => a.TutorialData = new AcademyTutorialData { CompletedTutorialNames = AcademyData.RequiredLicenseTutorials.Concat(AcademyData.SimpleTutorialPanels).ToArray() });
        CurrentProgressionData.RecordCompletedAdventure(TutorialAdventureId, TutorialDifficultyLevel, TutorialHeroId.AsArray());
        CurrentGameData.Clear();
        Message.Publish(new ExecuteAfterDelayRequested(delay, () => Message.Publish(new NavigateToNextTutorialFlow())));
    }
}
