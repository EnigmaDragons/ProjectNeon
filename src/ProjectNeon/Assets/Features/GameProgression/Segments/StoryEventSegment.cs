using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/StoryEvent")]
public class StoryEventSegment : StageSegment
{
    [SerializeField] private StoryEventPool2 events;
    [SerializeField] private AdventureProgress2 adventure;

    public override string Name => "Story Event";
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldAutoStart => false;
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.StoryEvent;
    public override Maybe<string> Corp => Maybe<string>.Missing();

    public override void Start()
    {
        var encountered = new HashSet<string>(adventure.FinishedStoryEvents);
        var possibleStoryEvents = events.AllEvents
            .Where(x =>
                x.StorySetting == adventure.CurrentChapter.StorySetting
                && !encountered.Contains(x.name))
            .ToArray();
        if (possibleStoryEvents.Any())
        {
            var storyEvent = possibleStoryEvents.Random();
            adventure.RecordEncounteredStoryEvent(storyEvent);
            Message.Publish(new BeginStoryEvent2(storyEvent));
        }
        else
        {
            Log.Warn($"Out of Story Events for {adventure.CurrentAdventure.Title} Chapter {adventure.CurrentChapterIndex}");
            Message.Publish(new ContinueTraveling());
        }
    }

    // Only Non-Deterministic for now
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}
