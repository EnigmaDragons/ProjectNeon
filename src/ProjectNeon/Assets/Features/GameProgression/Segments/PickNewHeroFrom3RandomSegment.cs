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
        var unlockedHeroes = library.UnlockedHeroes.ToList();
        currentParty.Heroes.ForEach(h => unlockedHeroes.Remove(h));
        var randomThree = unlockedHeroes.Shuffled().Take(3).ToArray();
        var prompt = currentParty.Heroes.Length == 0 ? "Choose Your Leader" : "Choose A New Squad Member";
        Message.Publish(new GetUserSelectedHero(prompt, randomThree, h => Message.Publish(new AddHeroToPartyRequested(h))));
    }

    public override Maybe<string> Detail { get; } = Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) => this;
}
