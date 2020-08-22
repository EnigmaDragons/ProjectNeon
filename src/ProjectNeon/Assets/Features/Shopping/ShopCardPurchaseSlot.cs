using System;
using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;

    private CardType _card;
    private int _price;
    
    public ShopCardPurchaseSlot Initialized(CardType c)
    {
        _card = c;
        _price = c.ShopPrice();
        var canAfford = party.Credits >= _price;
        cardPresenter.Set(c, canAfford ? PurchaseCard : (Action)(() => { }));
        if (!canAfford)
            cardPresenter.SetDisabled(true);
        costLabel.text = _price.ToString();
        return this;
    }

    private void PurchaseCard()
    {
        cardPresenter.Clear();
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_price);
        party.Cards.Add(_card);
    }
}
