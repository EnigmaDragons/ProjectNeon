using System;
using UnityEngine;

public static class TimelessResourceCalculator
{
    public static ResourceCalculations CalculateResources(this CardTypeData card, MemberState member) 
        => CalculateResources(card.Cost, card.Gain, member);
    public static ResourceCalculations CalculateResources(ResourceCost cost, ResourceCost gain, MemberState member)
        => new ResourceCalculations(cost.ResourceType, ResourcesPaid(cost, member), gain.ResourceType, ResourcesGained(gain, member), XAmountPaid(cost, member), XAmountPaid(cost, member));

    public static ResourceCalculations ClampResources(this ResourceCalculations calculations, Member member)
        => new ResourceCalculations(
            calculations.ResourcePaidType, 
            Mathf.Max(0, calculations.ResourcesPaid), 
            calculations.ResourceGainedType, 
            Mathf.Clamp(calculations.ResourcesGained, 0, member.ResourceMax(calculations.ResourceGainedType) - member.ResourceAmount(calculations.ResourceGainedType)),
            Math.Max(0, calculations.XAmount),
            Math.Max(0, calculations.XAmountPriceTag));

    private static int ResourcesPaid(ResourceCost cost, MemberState member)
    {
        if (cost == null || cost.ResourceType == null)
            return 0;

        return cost.PlusXCost
            ? cost.BaseAmount + Mathf.Max(0, member[cost.ResourceType] - cost.BaseAmount)
            : cost.BaseAmount;
    }

    private static int ResourcesGained(ResourceCost gain, MemberState member)
    {
        if (gain == null || gain.ResourceType == null)
            return 0;
        
        return gain.PlusXCost
            ? 999
            : gain.BaseAmount;
    }
    
    private static int XAmountPaid(ResourceCost cost, MemberState member)
    {
        if (cost == null || cost.ResourceType == null)
            return 0;

        return Mathf.Max(0, cost.PlusXCost
            ? member[cost.ResourceType] - cost.BaseAmount
            : cost.BaseAmount);
    }
}