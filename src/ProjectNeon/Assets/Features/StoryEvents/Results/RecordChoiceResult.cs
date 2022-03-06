using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Record Choice")]
public class RecordChoiceResult : StoryResult
{
    [SerializeField] private StringReference name;
    public bool state;
    
    public override int EstimatedCreditsValue => 0;
    
    public override void Apply(StoryEventContext ctx)
    {
        
    }

    public override void Preview() {}
}