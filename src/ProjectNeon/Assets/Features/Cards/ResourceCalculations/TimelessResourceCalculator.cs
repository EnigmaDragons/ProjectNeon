using System;
using UnityEngine;

public static class TimelessResourceCalculator
{
    public static ResourceCalculations CalculateResources(this CardTypeData card, MemberState member) 
        => CalculateResources(card.Cost, member);
    
    public static ResourceCalculations CalculateResources(IResourceAmount cost, MemberState member)
    {
        try
        {
            return new ResourceCalculations(cost.ResourceType.Name.Equals("PrimaryResource")
                    ? member.PrimaryResource
                    : cost.ResourceType,
                ResourcesPaid(cost, member),
                XAmountPaid(cost, member),
                XAmountPaid(cost, member));
        }
        catch (Exception e)
        {
            Log.Error(e);
            return new ResourceCalculations(new InMemoryResourceType(), 0, 0, 0);
        }
    }

    public static ResourceCalculations ClampResources(this ResourceCalculations calculations, Member member)
        => new ResourceCalculations(
            calculations.ResourcePaidType, 
            Mathf.Max(0, calculations.ResourcesPaid), 
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
    
    private static int XAmountPaid(IResourceAmount cost, MemberState member)
    {
        if (cost == null || cost.ResourceType == null)
            return 0;

        return Mathf.Max(0, cost.PlusXCost
            ? member[cost.ResourceType] - cost.BaseAmount
            : cost.BaseAmount);
    }
}