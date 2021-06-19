using System.Collections.Generic;
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
        var encountered = new HashSet<string>(adventure.FinishedStoryEvents);
        var storyEvent = events.AllEvents
            .Where(x => 
                x.StorySetting == adventure.CurrentChapter.StorySetting
                && !encountered.Contains(x.name))
            .Random();
        adventure.RecordEncounteredStoryEvent(storyEvent);
        Message.Publish(new BeginStoryEvent(storyEvent));
    }

    // Only Non-Deterministic for now
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx) => this;
}
