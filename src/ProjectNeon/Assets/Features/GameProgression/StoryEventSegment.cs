using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/StoryEvent")]
public class StoryEventSegment : StageSegment
{
    [SerializeField] private StoryEventPool events;
    [SerializeField] private AdventureProgress2 adventure;

    public override string Name => "Story Event";
    public override Maybe<string> Detail => Maybe<string>.Missing();

    public override void Start()
    {
        var storyEvent = events.AllEvents.Where(x => x.StorySetting == adventure.CurrentStage.StorySetting).Random();
        Message.Publish(new BeginStoryEvent(storyEvent));
    }

    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx)
    {
        var storyEvent = events.AllEvents.Where(x => x.StorySetting == adventure.CurrentStage.StorySetting).Random();
        return new InMemoryStageSegment(Name, () => Message.Publish(new BeginStoryEvent(storyEvent)), Maybe<string>.Missing());
    }
}
