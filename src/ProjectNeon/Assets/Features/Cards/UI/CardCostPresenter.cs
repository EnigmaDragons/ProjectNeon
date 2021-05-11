using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCostPresenter : MonoBehaviour
{
    [SerializeField] private GameObject costPanel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Image costResourceTypeIcon;
    
    // Pass a Maybe Card instead, perhaps
    public void Render(Card c, CardTypeData ct)
    {
        var cost = ct.Cost;
        var hasCost = !cost.ResourceType.Name.Equals("None") && cost.BaseAmount > 0 || cost.PlusXCost;
        costPanel.SetActive(hasCost);
        if (hasCost)
        {
            costLabel.text = CostLabel(c, cost);
            costResourceTypeIcon.sprite = c != null && cost.ResourceType.Name.Equals("PrimaryResource") 
                ? c.Owner.State.PrimaryResource.Icon 
                : cost.ResourceType.Icon;
        }
    }
    
    private string CostLabel(Card card, IResourceAmount cost)
    {
        var owner = card != null ? new Maybe<Member>(card.Owner) : Maybe<Member>.Missing();
        var numericAmount = cost.BaseAmount.ToString();
        // Non-X Cost Cards
        if (!cost.PlusXCost)
            return numericAmount;

        // X Cost Cards
        if (owner.IsMissing)
            return $"{numericAmount}+X".Replace("0+", "");
        else
            return card.LockedXValue.Select(
                r => $"{card.Cost.BaseAmount}+{r.Amount}".Replace("0+", ""),
                () => $"{card.Cost.BaseAmount}+{card.Owner.CalculateResources(card.Type).XAmountPriceTag}".Replace("0+", ""));
    }
}
