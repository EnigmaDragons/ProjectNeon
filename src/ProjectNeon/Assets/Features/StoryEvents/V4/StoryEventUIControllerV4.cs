using UnityEngine;

public class StoryEventUIControllerV4 : OnMessage<BeginStoryEvent2, MarkStoryEventCompleted, PrepCombatForAfterEvent>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private StoryEventPresenterV4 presenter;

    private EnterSpecificBattle battle;
    
    protected override void Execute(BeginStoryEvent2 msg)
    {
        presenter.Present(msg.StoryEvent);
        this.ExecuteAfterTinyDelay(() => ui.SetActive(true));
    }

    protected override void Execute(MarkStoryEventCompleted msg)
    {
        if (battle != null)
        {
            Message.Publish(new FinishedStoryEvent { ShouldNotContinue = true });
            ui.SetActive(false); 
            Message.Publish(battle);
            battle = null;
        }
        else
        {
            Message.Publish(new FinishedStoryEvent());
            ui.SetActive(false);
        }
    }

    protected override void Execute(PrepCombatForAfterEvent msg)
    {
        battle = msg.Battle;
    }
}