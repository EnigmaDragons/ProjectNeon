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
        var existingHeroes = currentParty.Heroes.ToArray();
        var allOptions = library.UnlockedHeroes.ToList();
        existingHeroes.ForEach(h => allOptions.Remove(h));
        
        var currentArchs = currentParty.Heroes.SelectMany(h => h.Archetypes).ToHashSet();
        var preferredSelection = allOptions.Where(h => !h.Archetypes.Any(a => currentArchs.Contains(a))).ToList();
        var optimizedSelection = preferredSelection.Count() >= 3 ? preferredSelection : allOptions;
        
        var randomThree = optimizedSelection.Shuffled().Take(3).ToArray();
        var prompt = currentParty.Heroes.Length == 0 ? "Choose Your Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, randomThree, h =>
        {
            AllMetrics.PublishHeroSelected(h.Name, randomThree.Select(x => x.Name).ToArray(), existingHeroes.Select(x => x.Name).ToArray());
            Message.Publish(new AddHeroToPartyRequested(h));
        }));
    }

    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
