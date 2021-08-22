using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "StoryEvent/Results/Combat")]
public class CombatResult : StoryResult
{
    [SerializeField] private GameObject battleField;
    [SerializeField] private bool isElite;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] private int estimatedCreditsValue;
    
    public override int EstimatedCreditsValue => estimatedCreditsValue;
    
    public override void Apply(StoryEventContext ctx)
    {
        Message.Publish(new PrepCombatForAfterEvent(new EnterSpecificBattle(battleField, isElite, 
            enemies.Select(x => x.ForStage(ctx.Adventure.CurrentChapterNumber)).ToArray(), true)));
    }

    public override void Preview()
    {
        Message.Publish(new ShowTextResultPreview { IsReward = EstimatedCreditsValue > 0, Text = $"Start a fight" });
    }
}