
public class TutorialTrackerReactiveSystem : OnMessage<ShowTutorialSlideshowIfNeeded, HideTutorial>
{
    protected override void Execute(ShowTutorialSlideshowIfNeeded msg)
    {
        if (TutorialTracker.ShouldShowTutorial(msg.Tutorial.TutorialName))
            Message.Publish(new ShowTutorialSlideshow(msg.Tutorial));
    }

    protected override void Execute(HideTutorial msg) 
        => msg.CompletedTutorialName.IfPresent(TutorialTracker.RecordTutorialCompleted);
}