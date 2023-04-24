using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCostPresenter : MonoBehaviour
{
    [SerializeField] private GameObject costPanel;
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI costLabel;
    [SerializeField] private Image costResourceTypeIcon;
    [SerializeField] private ResourceSpriteMap resourceIcons;
    
    public void Render(Maybe<Card> c, CardTypeData ct, IResourceType primaryResourceType, bool forceShowXcostAsX = false)
    {
        try
        {
            var cost = ct.Cost;
            var hasCost = !cost.ResourceType.Name.Equals("None") && (cost.BaseAmount > 0 || cost.PlusXCost);
            costPanel.SetActive(hasCost);
            if (hasCost)
            {
                costLabel.text = CostLabel(c, cost, forceShowXcostAsX);
                costResourceTypeIcon.sprite = c.IsPresent && cost.ResourceType.Name.Equals("PrimaryResource")
                    ? resourceIcons.Get(c.Value.Owner.State.PrimaryResource.Name)
                    : cost.ResourceType.Name.Equals("PrimaryResource")
                        ? resourceIcons.Get(primaryResourceType.Name)
                        : resourceIcons.Get(cost.ResourceType.Name);
            }
        }
        catch (Exception e)
        {
            Log.Error(e);
            costPanel.SetActive(false);
        }
    }
    
    private string CostLabel(Maybe<Card> maybeCard, IResourceAmount cost, bool forceShowXcostAsX)
    {
        var numericAmount = cost.BaseAmount.ToString();
        // Non-X Cost Cards
        if (!cost.PlusXCost)
            return numericAmount;

        // X Cost Cards
        return $"{numericAmount}+X".Replace("0+", "");
        
        // Old X-Cost Cards Split
        // if (maybeCard.IsMissing || forceShowXcostAsX)
        //     return $"{numericAmount}+X".Replace("0+", "");
        // else
        // {
        //     var card = maybeCard.Value;
        //     return card.LockedXValue.Select(
        //         r => $"{card.Cost.BaseAmount}+{r.Amount}".Replace("0+", string.Empty),
        //         () => $"{card.Cost.BaseAmount}+{card.Owner.CalculateResources(card.Type).XAmountPriceTag}".Replace("0+", string.Empty));
        // }
    }
}
