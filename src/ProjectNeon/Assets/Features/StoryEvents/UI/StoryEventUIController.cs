
using UnityEngine;

public class StoryEventUIController : OnMessage<BeginStoryEvent, MarkStoryEventCompleted>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private StoryEventPresenter presenter;
    
    protected override void Execute(BeginStoryEvent msg)
    {
        presenter.Present(msg.StoryEvent);
        ui.SetActive(true);
    }

    protected override void Execute(MarkStoryEventCompleted msg)
    {
        ui.SetActive(false);
    }
}
