using System;
using UnityEngine;

public static class TimelessResourceCalculator
{
    public static ResourceCalculations CalculateResources(this CardTypeData card, MemberState member) 
        => CalculateResources(card.Cost, new InMemoryResourceAmount(0), member);
    public static ResourceCalculations CalculateResources(IResourceAmount cost, IResourceAmount gain, MemberState member)
        => new ResourceCalculations(cost.ResourceType.Name.Equals("PrimaryResource") ? member.PrimaryResource : cost.ResourceType, 
            ResourcesPaid(cost, member), gain.ResourceType, ResourcesGained(gain, member), XAmountPaid(cost, member), XAmountPaid(cost, member));

    public static ResourceCalculations ClampResources(this ResourceCalculations calculations, Member member)
        => new ResourceCalculations(
            calculations.ResourcePaidType, 
            Mathf.Max(0, calculations.ResourcesPaid), 
            calculations.ResourceGainedType, 
            calculations.ResourceGainedType.Name == "Creds" 
                ? calculations.ResourcesGained
                : Mathf.Clamp(calculations.ResourcesGained, 0, member.ResourceMax(calculations.ResourceGainedType) - member.ResourceAmount(calculations.ResourceGainedType)),
            Math.Max(0, calculations.XAmount),
            Math.Max(0, calculations.XAmountPriceTag));

    private static int ResourcesPaid(IResourceAmount cost, MemberState member)
    {
        if (cost == null || cost.ResourceType == null)
            return 0;

        return cost.PlusXCost
            ? cost.BaseAmount + Mathf.Max(0, member[cost.ResourceType] - cost.BaseAmount)
            : cost.BaseAmount;
    }

    private static int ResourcesGained(IResourceAmount gain, MemberState member)
    {
        if (gain == null || gain.ResourceType == null)
            return 0;
        
        return gain.PlusXCost
            ? 999
            : gain.BaseAmount;
    }
    
    private static int XAmountPaid(IResourceAmount cost, MemberState member)
    {
        if (cost == null || cost.ResourceType == null)
            return 0;

        return Mathf.Max(0, cost.PlusXCost
            ? member[cost.ResourceType] - cost.BaseAmount
            : cost.BaseAmount);
    }
}