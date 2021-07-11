using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/ClinicSegment")]
public class ClinicSegment : StageSegment
{
    public override string Name => "Clinic";
    public override void Start() => Message.Publish(new ToggleClinic());
    public override Maybe<string> Detail => Maybe<string>.Missing();
    public override IStageSegment GenerateDeterministic(AdventureGenerationContext ctx, MapNode3 mapData) 
        => new GeneratedClinicSegment(mapData.Corp);
}
