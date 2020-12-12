using UnityEngine;

[CreateAssetMenu(menuName = "Adventure/ClinicSegment")]
public class ClinicSegment : StageSegment
{
    public override string Name => "Clinic";
    public override void Start() => Message.Publish(new ToggleClinic());
}
