using System;
using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;

    private Card _card;
    private int _price;
    private bool _purchased;

    private void AfterEnable()
    {
        UpdateAffordability();
    }
    
    public ShopCardPurchaseSlot Initialized(Card c)
    {
        soldVisual.SetActive(false);
        _card = c;
        _price = c.CardShopPrice();
        costLabel.text = _price.ToString();
        UpdateAffordability();
        return this;
    }

    private void UpdateAffordability()
    {
        if (_card == null || _purchased)
            return;
        
        var canAfford = party.Credits >= _price;
        var cardAction = canAfford ? PurchaseCard : (Action) (() => { });
        cardPresenter.Set(_card, cardAction);
        
        if (!canAfford)
            cardPresenter.SetDisabled(true);
    }

    private void PurchaseCard()
    {
        _purchased = true;
        cardPresenter.Clear();
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_price);
        party.Add(_card.BaseType);
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();
}
