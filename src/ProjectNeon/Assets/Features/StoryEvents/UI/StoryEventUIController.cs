using UnityEngine;

public class StoryEventUIController : OnMessage<BeginStoryEvent2, MarkStoryEventCompleted>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private StoryEventPresenter2 presenter;

    protected override void Execute(BeginStoryEvent2 msg)
    {
        presenter.Present(msg.StoryEvent);
        ui.SetActive(true);
    }

    protected override void Execute(MarkStoryEventCompleted msg)
    {
        Message.Publish(new FinishedStoryEvent());
        Message.Publish(new AutoSaveRequested());
        ui.SetActive(false);
    }
}
