using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/ClinicSegment")]
public class ClinicSegment : StageSegment
{
    [SerializeField] private StringReference corp;
    [SerializeField] private bool isTutorial;
    
    public override string Name => "Clinic";
    public override bool ShouldCountTowardsEnemyPowerLevel => false;
    public override bool ShouldAutoStart => false;
    public override void Start() => Message.Publish(new ToggleClinic { CorpName = corp, IsTutorial = isTutorial });
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override MapNodeType MapNodeType => MapNodeType.Clinic;
    public override Maybe<string> Corp => Maybe<string>.Present(corp);
    
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) 
        => new GeneratedClinicSegment(mapData.Corp, isTutorial);
    public override bool ShouldSpawnThisOnMap(CurrentAdventureProgress p) => true;
}
