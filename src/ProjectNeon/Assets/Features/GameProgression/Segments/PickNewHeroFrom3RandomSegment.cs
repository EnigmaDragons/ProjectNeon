using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/HeroPick3")]
public class PickNewHeroFrom3RandomSegment : StageSegment, ILocalizeTerms
{
    [SerializeField] private Library library;
    [SerializeField] private Party currentParty;
    [SerializeField] private CurrentAdventure currentAdventure;
    [SerializeField] private DeterminedNodeInfo determinedNodeInfo; 

    public override string Name => $"Party Change Event";
    public override void Start()
    {
        if (determinedNodeInfo.PickHeroes.IsMissing)
        {
            determinedNodeInfo.PickHeroes = GetFeatureHeroOptions(library, currentParty.Heroes, currentAdventure.Adventure);
            Message.Publish(new SaveDeterminationsRequested());
        }
        var prompt = currentParty.Heroes.Length == 0 ? "Menu/ChooseLeader" : "Menu/ChooseMember";
        Message.Publish(new GetUserSelectedHero(prompt, determinedNodeInfo.PickHeroes.Value, h =>
        {
            AllMetrics.PublishHeroSelected(h.NameTerm().ToEnglish(), determinedNodeInfo.PickHeroes.Value.Select(x => x.NameTerm().ToEnglish()).ToArray(), currentParty.Heroes.Select(x => x.NameTerm().ToEnglish()).ToArray());
            determinedNodeInfo.PickHeroes = Maybe<BaseHero[]>.Missing();
            Message.Publish(new AddHeroToPartyRequested(h));        
            if (currentAdventure != null && currentParty.Heroes.Length == currentAdventure.Adventure.PartySize && currentParty.Heroes.All(h => h.Sex == CharacterSex.Female))
                Achievements.Record(Achievement.MiscGirlPower);
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

    public static BaseHero[] GetFeatureHeroOptions(Library library, BaseHero[] currentHeroes, Adventure adventure)
    {
        var existingHeroes = currentHeroes.ToArray();
        var allOptions = library.UnlockedHeroes.Where(x => x.AdventuresPlayedBeforeUnlocked == 0 || CurrentProgressionData.Data.HasShownUnlockForHeroId(x.Id)).ToList();
        existingHeroes.ForEach(h => allOptions.Remove(h));
        adventure.BannedHeroes.ForEach(h => allOptions.Remove(h));
        if (currentHeroes.Length == 0)
            adventure.BannedLeaders.ForEach(h => allOptions.Remove(h));

        var currentArchs = currentHeroes.SelectMany(h => h.Archetypes).ToHashSet();
        var preferredSelection = allOptions.Where(h => !h.Archetypes.Any(a => currentArchs.Contains(a))).ToList();
        var optimizedSelection = preferredSelection.Count() >= 3 ? preferredSelection : allOptions;
        var randomThree = optimizedSelection.Shuffled(new DeterministicRng(ConsumableRngSeed.Consume())).Take(3).ToArray();
        
        var maybeFeaturedHero = library.MaybeFeaturedHero;
        if (maybeFeaturedHero.IsMissing && allOptions.Any(x => CurrentProgressionData.Data.RunsFinished == x.AdventuresPlayedBeforeUnlocked))
            maybeFeaturedHero = allOptions.First(x => CurrentProgressionData.Data.RunsFinished == x.AdventuresPlayedBeforeUnlocked);
        var featuredThree = randomThree;
        if (maybeFeaturedHero.IsPresentAnd(h => !currentHeroes.Contains(h) 
                                                && !randomThree.Contains(h) 
                                                && h.Archetypes.None(a => currentArchs.Contains(a))))
            featuredThree = new [] {maybeFeaturedHero.Value, randomThree[0], randomThree[1]};

        featuredThree = featuredThree.OrderBy(h => h.ComplexityRating).ToArray();
        return featuredThree;
    }

    public string[] GetLocalizeTerms()
        => new[] { "Menu/ChooseLeader", "Menu/ChooseMember" };
}
