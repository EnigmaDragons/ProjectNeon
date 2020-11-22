using System;

[Serializable]
public class StoryEventCost
{
    public StoryEventCostType CostType;
    public int CostAmount;

    public string CostDescription() => CostAmount != 0 ? $"[Pay {CostAmount} {CostType}]" : "";

    public bool CanAfford(StoryEventContext ctx)
    {
        if (CostType == StoryEventCostType.Credits)
            return ctx.Party.Credits >= CostAmount;
        return false;
    }
    
    public void Apply(StoryEventContext ctx)
    {
        if (CostType == StoryEventCostType.Credits)
            ctx.Party.UpdateCreditsBy(-CostAmount);
        else
            Log.Error($"Story Event: Unknown how to process Cost Type {CostType}");
    }
}
