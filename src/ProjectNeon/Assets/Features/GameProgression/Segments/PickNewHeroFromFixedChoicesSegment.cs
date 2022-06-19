﻿using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/FixedHeroPick")]
public class PickNewHeroFromFixedChoicesSegment: StageSegment
{
    [SerializeField] private BaseHero[] options;
    [SerializeField] private Party currentParty;

    public override string Name => $"Party Change Event";
    public override void Start()
    {
        var existingHeroes = currentParty.Heroes.ToArray();
        var prompt = currentParty.Heroes.Length == 0 ? "Choose Your Mission Squad Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, options, h =>
        {
            AllMetrics.PublishHeroSelected(h.Name, options.Select(x => x.Name).ToArray(), existingHeroes.Select(x => x.Name).ToArray());
            Message.Publish(new AddHeroToPartyRequested(h));
            Async.ExecuteAfterDelay(0.5f, () => Message.Publish(new ToggleNamedTarget("HeroSelectionView")));
        }));
    }

    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldAutoStart => true;
    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.Unknown;
    public override Maybe<string> Corp => Maybe<string>.Missing();
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}