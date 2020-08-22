using System;
using TMPro;
using UnityEngine;

public sealed class ShopEquipmentPurchaseSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private EquipmentPresenter equipmentPresenter;
    [SerializeField] private GameObject soldVisual;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private GameObject darken;

    private Equipment _equipment;
    
    public ShopEquipmentPurchaseSlot Initialized(Equipment e)
    {
        _equipment = e;
        var canAfford = party.Credits >= e.Price;
        equipmentPresenter.Initialized(e, canAfford ? Purchase : (Action)(() => { }));
        if (!canAfford)
            darken.SetActive(true);
        costLabel.text = e.Price.ToString();
        return this;
    }

    private void Purchase()
    {
        equipmentPresenter.gameObject.SetActive(false);
        soldVisual.SetActive(true);
        party.UpdateCreditsBy(-_equipment.Price);
        party.Add(_equipment);
    }
}