
public class HideTutorial
{
    public Maybe<string> CompletedTutorialName { get; }

    public HideTutorial(Maybe<string> completedTutorialName) => CompletedTutorialName = completedTutorialName;
}
