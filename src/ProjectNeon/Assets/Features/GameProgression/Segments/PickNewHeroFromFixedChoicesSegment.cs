using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName= "Adventure/FixedHeroPick")]
public class PickNewHeroFromFixedChoicesSegment: StageSegment, ILocalizeTerms
{
    [SerializeField] private BaseHero[] options;
    [SerializeField] private Party currentParty;

    public override string Name => $"Party Change Event";
    public override void Start()
    {
        var existingHeroes = currentParty.Heroes.ToArray();
        var prompt = currentParty.Heroes.Length == 0 ? "Menu/ChooseLeader" : "Menu/ChooseMember";
        Message.Publish(new GetUserSelectedHero(prompt, options, h =>
        {
            AllMetrics.PublishHeroSelected(h.NameTerm().ToEnglish(), options.Select(x => x.NameTerm().ToEnglish()).ToArray(), existingHeroes.Select(x => x.NameTerm().ToEnglish()).ToArray());
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

    public string[] GetLocalizeTerms()
        => new[] {"Menu/ChooseLeader", "Menu/ChooseMember"};
}