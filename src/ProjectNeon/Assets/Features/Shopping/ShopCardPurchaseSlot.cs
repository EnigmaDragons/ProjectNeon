using System;
using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : OnMessage<PartyAdventureStateChanged>
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
        costLabel.text = _price.ToString();
        UpdateAffordability();
        return this;
    }

    private void UpdateAffordability()
    {
        var canAfford = party.Credits >= _price;
        cardPresenter.Set(_card, canAfford ? PurchaseCard : (Action)(() => { }));
        if (!canAfford)
            cardPresenter.SetDisabled(true);
    }

    private void PurchaseCard()
    {
        cardPresenter.Clear();
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_price);
        party.Cards.Add(_card);
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();
}
