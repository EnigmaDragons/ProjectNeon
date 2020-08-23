using UnityEngine;

public sealed class ShopPresenter : MonoBehaviour
{
    [SerializeField] private ShopCardPool cards;
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopCardPurchaseSlot cardPurchasePrototype;
    [SerializeField] private ShopEquipmentPurchaseSlot equipmentPurchasePrototype;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private GameObject equipmentParent;
     
    private ShopSelection _selection;

    private void Awake()
    {
        foreach (Transform c in cardParent.transform) 
            Destroy(c.gameObject);
        foreach (Transform c in equipmentParent.transform) 
            Destroy(c.gameObject);
    }
    
    private void Start() => GetMoreInventory();

    public void GetMoreInventory()
    {
        foreach (Transform c in cardParent.transform) 
            Destroy(c.gameObject);
        foreach (Transform c in equipmentParent.transform) 
            Destroy(c.gameObject);
        _selection = new ShopSelectionPicker()
            .GenerateSelection(cards, equipment, party);
        _selection.Cards.ForEach(c => 
            Instantiate(cardPurchasePrototype, cardParent.transform)
                .Initialized(c));
        _selection.Equipment.ForEach(e => 
            Instantiate(equipmentPurchasePrototype, equipmentParent.transform)
                .Initialized(e));
    }
}
