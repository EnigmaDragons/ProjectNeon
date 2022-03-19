using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/RemoveHeroFromParty")]
public class RemoveHeroFromPartySegment : StageSegment
{
    [SerializeField] private BaseHero hero;

    public override string Name => $"Party Change Event";
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldAutoStart => false;
    public override void Start()
    {
        Message.Publish(new RemoveHeroFromPartyRequested(hero));
        Message.Publish(new NodeFinished());
    } 

    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.Unknown;
    public override Maybe<string> Corp => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
