using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Record Choice")]
public class RecordChoiceResult : StoryResult
{
    [SerializeField] private StringReference name;
    public bool state;
    
    public override int EstimatedCreditsValue => 0;
    
    public override void Apply(StoryEventContext ctx) => ctx.Adventure.SetStoryState(name.Value, state);

    public override void Preview() {}
}