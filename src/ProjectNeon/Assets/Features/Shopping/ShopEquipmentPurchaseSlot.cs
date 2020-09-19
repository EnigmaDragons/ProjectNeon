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

    private Equipment _equipment;
    private bool _purchased;

    private void AfterEnable()
    {
        UpdateAffordability();
    }

    public ShopEquipmentPurchaseSlot Initialized(Equipment e)
    {
        soldVisual.SetActive(false);
        _equipment = e;
        UpdateAffordability();
        costLabel.text = e.Price.ToString();
        return this;
    }

    private void UpdateAffordability()
    {
        if (_equipment == null || _purchased)
            return;
        
        var canAfford = party.Credits >= _equipment.Price;
        equipmentPresenter.Initialized(_equipment, canAfford ? Purchase : (Action) (() => { }));
        if (!canAfford)
            darken.SetActive(true);
    }

    private void Purchase()
    {
        _purchased = true;
        equipmentPresenter.gameObject.SetActive(false);
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_equipment.Price);
        party.Add(_equipment);
    }

    protected override void Execute(PartyAdventureStateChanged msg) => UpdateAffordability();
}
