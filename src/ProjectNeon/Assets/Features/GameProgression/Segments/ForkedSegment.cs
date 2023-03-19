using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/ForkedSegment")]
public class ForkedSegment : StageSegment
{
    [SerializeField] private CurrentAdventureProgress currentAdventure;
    [SerializeField] private StageSegment defaultSegment;
    [SerializeField] private StageSegment conditionalSegment;
    
    public StringReference[] RequiredStates = Array.Empty<StringReference>();
    public StringReference[] ForbiddenStates = Array.Empty<StringReference>();
    public bool Or;

    public override string Name => Pick().Name;
    public override bool ShouldCountTowardsEnemyPowerLevel => Pick().ShouldCountTowardsEnemyPowerLevel;
    public override void Start() => Pick().Start();
    public override Maybe<string> Detail => Pick().Detail;
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => Pick().GenerateDeterministic(ctx, mapData);
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => Pick().ShouldSpawnThisOnMap(p);
    public override MapNodeType MapNodeType => Pick().MapNodeType;
    public override Maybe<string> Corp => Pick().Corp;
    public override bool ShouldAutoStart => Pick().ShouldAutoStart;

    private StageSegment Pick() 
        => ShouldSkipConditional(s => currentAdventure.AdventureProgress.IsTrue(s))
            ? defaultSegment
            : conditionalSegment;

    private bool ShouldSkipConditional(Func<string, bool> storyState)
        => ForbiddenStates.Any(x => storyState(x.Value))
           || (Or && RequiredStates.None(x => storyState(x.Value)))
           || (!Or && RequiredStates.Any(x => !storyState(x.Value)));
}