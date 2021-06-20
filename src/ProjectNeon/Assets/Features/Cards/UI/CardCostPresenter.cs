using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCostPresenter : MonoBehaviour
{
    [SerializeField] private GameObject costPanel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private Image costResourceTypeIcon;
    
    public void Render(Maybe<Card> c, CardTypeData ct)
    {
        var cost = ct.Cost;
        var hasCost = !cost.ResourceType.Name.Equals("None") && cost.BaseAmount > 0 || cost.PlusXCost;
        costPanel.SetActive(hasCost);
        if (hasCost)
        {
            costLabel.text = CostLabel(c, cost);
            costResourceTypeIcon.sprite = c.IsPresent && cost.ResourceType.Name.Equals("PrimaryResource") 
                ? c.Value.Owner.State.PrimaryResource.Icon 
                : cost.ResourceType.Icon;
        }
    }
    
    private string CostLabel(Maybe<Card> maybeCard, IResourceAmount cost)
    {
        var numericAmount = cost.BaseAmount.ToString();
        // Non-X Cost Cards
        if (!cost.PlusXCost)
            return numericAmount;

        // X Cost Cards
        if (maybeCard.IsMissing)
            return $"{numericAmount}+X".Replace("0+", "");
        else
        {
            var card = maybeCard.Value;
            return card.LockedXValue.Select(
                r => $"{card.Cost.BaseAmount}+{r.Amount}".Replace("0+", ""),
                () => $"{card.Cost.BaseAmount}+{card.Owner.CalculateResources(card.Type).XAmountPriceTag}".Replace("0+", ""));
        }
    }
}
