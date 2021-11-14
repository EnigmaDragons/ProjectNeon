using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/RemoveHeroFromParty")]
public class RemoveHeroFromPartySegment : StageSegment
{
    [SerializeField] private BaseHero hero;

    public override string Name => $"Party Change Event";

    public override void Start()
    {
        Message.Publish(new RemoveHeroFromPartyRequested(hero));
        Message.Publish(new NodeFinished());
    } 

    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
