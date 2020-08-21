using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject soldVisual;
    
    public ShopCardPurchaseSlot Initialized(CardType c)
    {
        cardPresenter.Set(c, PurchaseCard);
        costLabel.text = c.ShopPrice().ToString();
        return this;
    }

    private void PurchaseCard()
    {
        cardPresenter.Clear();
        soldVisual.SetActive(true);
        // TODO: Implement Purchase Behavior
    }
}
