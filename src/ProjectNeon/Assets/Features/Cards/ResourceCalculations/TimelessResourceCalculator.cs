using System;
using UnityEngine;

public static class TimelessResourceCalculator
{
    public static ResourceCalculations CalculateResources(this CardTypeData card, MemberState member)
    {
        if (card == null)
        {
            Log.Error("Calculate Resources was called with a Null Card");
            return ResourceCalculations.Free;
        }
        try
        {
            if (card.Cost.BaseAmount == 0 && !card.Cost.PlusXCost)
                return ResourceCalculations.Free;
            return CalculateResources(card.Name, card.Cost, member);
        }
        catch (Exception e)
        {
            Log.Error(e);
            return ResourceCalculations.Free;
        }
    }

    private static ResourceCalculations CalculateResources(string cardName, IResourceAmount cost, MemberState member)
    {
        try
        {
            if (cost == null || cost.ResourceType == null)
            {
                Log.Warn($"{cardName} has invalid/null Cost.");
                return new ResourceCalculations(new InMemoryResourceType(), 0, 0, 0);
            }

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
            return ResourceCalculations.Free;
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