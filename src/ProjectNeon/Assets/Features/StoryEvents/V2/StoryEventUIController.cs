using UnityEngine;

public class StoryEventUIController : OnMessage<BeginStoryEvent2, MarkStoryEventCompleted, PrepCombatForAfterEvent>
{
    [SerializeField] private GameObject ui;
    [SerializeField] private StoryEventPresenter2 presenter;
    [SerializeField] private CurrentGameMap3 map;

    private EnterSpecificBattle battle;
    
    protected override void Execute(BeginStoryEvent2 msg)
    {
        presenter.Present(msg.StoryEvent);
        ui.SetActive(true);
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
            Message.Publish(new AutoSaveRequested());
            ui.SetActive(false);   
        }
    }

    protected override void Execute(PrepCombatForAfterEvent msg)
    {
        battle = msg.Battle;
    }
}
