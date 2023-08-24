using System;
using TMPro;
using UnityEngine;

public sealed class ShopCardPurchaseSlot : OnMessage<PartyAdventureStateChanged, SetSuperFocusBuyControl>
{
    [SerializeField, NoLocalizationNeeded] private TextMeshProUGUI costLabel;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private CurrentGlobalEffects globalEffects;
    [SerializeField] private GameObject superFocus;
    [SerializeField] private bool highlightOnHover = false;

    private Card _card;
    private int _price;
    private bool _purchased;

    private bool CanAfford => _price <= party.Credits;
    
    protected override void Execute(SetSuperFocusBuyControl msg)
    {
        if (!msg.Enabled)
            superFocus.SetActive(false);
        else if (party.Credits >= _price)
            superFocus.SetActive(true);
    }

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
        costLabel.text = _price.ToString() + 0;
        UpdateAffordability();
        gameObject.SetActive(true);
        return this;
    }

    private void UpdateAffordability()
    {
        if (_card == null || _purchased)
            return;
        
        var canAfford = CanAfford;
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
        party.Add(_card.CardTypeOrNothing.Value);
        Message.Publish(new CardPurchased(_card, transform));
        Message.Publish(new SetSuperFocusBuyControl(false));
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();

    public void HoverEnter()
    {
        if (highlightOnHover && !_purchased && CanAfford)
        {
            cardPresenter.SetHoverHighlight(true);
            Message.Publish(new ShopCardHovered(transform));
        }
    }

    public void HoverExit() => cardPresenter.SetHoverHighlight(false);
}
