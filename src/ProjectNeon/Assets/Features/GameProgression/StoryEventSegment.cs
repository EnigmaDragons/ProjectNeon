using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/StoryEvent")]
public class StoryEventSegment : StageSegment
{
    [SerializeField] private StoryEventPool events;
    [SerializeField] private AdventureProgress adventure;
    
    public override string Name => "Story Event";
    
    public override void Start()
    {
        var storyEvent = events.AllEvents.Where(x => x.StorySetting == adventure.CurrentStage.StorySetting).Random();
        Message.Publish(new BeginStoryEvent(storyEvent));
    }
}
