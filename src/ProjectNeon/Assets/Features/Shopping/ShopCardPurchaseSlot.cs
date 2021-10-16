using System;
using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CurrentGlobalEffects globalEffects;

    private Card _card;
    private int _price;
    private bool _purchased;

    protected override void AfterEnable()
    {
        UpdateAffordability();
        Log.ErrorIfNull(globalEffects, nameof(ShopCardPurchaseSlot), nameof(globalEffects));
    }
    
    public ShopCardPurchaseSlot Initialized(Card c)
    {
        soldVisual.SetActive(false);
        _card = c;
        _price = c.Rarity.CardShopPrice(globalEffects?.CardShopPriceFactor ?? 1f);
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
        Message.Publish(new CardPurchased(transform));
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();
}
