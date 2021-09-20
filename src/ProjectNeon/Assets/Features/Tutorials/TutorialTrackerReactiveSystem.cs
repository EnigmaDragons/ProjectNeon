using UnityEngine;

public class TutorialTrackerReactiveSystem : OnMessage<ShowTutorialSlideshowIfNeeded, ShowTutorialByNameIfNeeded, HideTutorial>
{
    [SerializeField] private AllTutorialSlideshows tutorialSlideshows;
    [SerializeField] private bool loggingEnabled = false;
    
    protected override void Execute(ShowTutorialSlideshowIfNeeded msg)
    {
        if (loggingEnabled)
            Log.Info($"Requested Show Tutorial {msg.Tutorial.TutorialName}");
        if (TutorialTracker.ShouldShowTutorial(msg.Tutorial.TutorialName))
            Message.Publish(new ShowTutorialSlideshow(msg.Tutorial));
    }

    protected override void Execute(ShowTutorialByNameIfNeeded msg)
    {
        var maybeTutorial = tutorialSlideshows.GetByName(msg.TutorialName);
        if (maybeTutorial.IsMissing)
            Log.Warn($"No Tutorial named {msg.TutorialName} found in All Tutorials");
        else
            Message.Publish(new ShowTutorialSlideshowIfNeeded(maybeTutorial.Value));
    }

    protected override void Execute(HideTutorial msg) 
        => msg.CompletedTutorialName.IfPresent(TutorialTracker.RecordTutorialCompleted);
}
