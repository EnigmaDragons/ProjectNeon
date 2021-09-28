using System;
using TMPro;
using UnityEngine;

public sealed class ShopEquipmentPurchaseSlot : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private EquipmentPresenter equipmentPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject darken;
    [SerializeField] private Color discountTextColor = Color.green;
    [SerializeField] private Color markupTextColor = Color.red;
    
    private Equipment _equipment;
    private float _priceFactor;
    private int _price;
    private bool _purchased;

    protected override void AfterEnable()
    {
        UpdateAffordability();
    }

    public ShopEquipmentPurchaseSlot Initialized(Equipment e) => Initialized(e, 1f);
    
    public ShopEquipmentPurchaseSlot Initialized(Equipment e, float priceFactor)
    {
        soldVisual.SetActive(false);
        _equipment = e;
        _priceFactor = priceFactor;
        _price = (_equipment.Price * _priceFactor).CeilingInt();
        UpdateAffordability();
        costLabel.text = _price.ToString();
        costLabel.color = _priceFactor == 1f ? Color.white : _priceFactor > 1f ? markupTextColor : discountTextColor;
        return this;
    }

    private void UpdateAffordability()
    {
        if (_equipment == null || _purchased)
            return;
        
        var canAfford = party.Credits >= _price;
        equipmentPresenter.Initialized(_equipment, canAfford ? Purchase : (Action) (() => { }));
        if (!canAfford)
            darken.SetActive(true);
    }

    private void Purchase()
    {
        _purchased = true;
        equipmentPresenter.gameObject.SetActive(false);
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_price);
        party.Add(_equipment);
        Message.Publish(new EQpurchased(transform));
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();
}
