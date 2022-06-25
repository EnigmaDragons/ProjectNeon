using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/HeroPick3")]
public class PickNewHeroFrom3RandomSegment : StageSegment
{
    [SerializeField] private Library library;
    [SerializeField] private Party currentParty;

    public override string Name => $"Party Change Event";
    public override void Start()
    {
        var featuredThree = GetFeatureHeroOptions(library, currentParty.Heroes);
        var prompt = currentParty.Heroes.Length == 0 ? "Choose Your Mission Squad Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, featuredThree, h =>
        {
            AllMetrics.PublishHeroSelected(h.Name, featuredThree.Select(x => x.Name).ToArray(), currentParty.Heroes.Select(x => x.Name).ToArray());
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

    public static BaseHero[] GetFeatureHeroOptions(Library library, BaseHero[] currentHeroes)
    {
        var existingHeroes = currentHeroes.ToArray();
        var allOptions = library.UnlockedHeroes.ToList();
        existingHeroes.ForEach(h => allOptions.Remove(h));
        
        var currentArchs = currentHeroes.SelectMany(h => h.Archetypes).ToHashSet();
        var preferredSelection = allOptions.Where(h => !h.Archetypes.Any(a => currentArchs.Contains(a))).ToList();
        var optimizedSelection = preferredSelection.Count() >= 3 ? preferredSelection : allOptions;
        var randomThree = optimizedSelection.Shuffled().Take(3).ToArray();
        
        var maybeFeaturedHero = library.MaybeFeaturedHero;
        var featuredThree = randomThree;
        if (maybeFeaturedHero.IsPresentAnd(h => !currentHeroes.Contains(h) 
                                                && !randomThree.Contains(h) 
                                                && h.Archetypes.None(a => currentArchs.Contains(a))))
            featuredThree = new [] {maybeFeaturedHero.Value, randomThree[0], randomThree[1]};

        featuredThree = featuredThree.OrderBy(h => h.ComplexityRating).ToArray();
        return featuredThree;
    }
}
