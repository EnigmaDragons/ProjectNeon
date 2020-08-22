using UnityEngine;

public sealed class ShopPresenter : MonoBehaviour
{
    [SerializeField] private ShopCardPool cards;
    [SerializeField] private EquipmentPool equipment;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopCardPurchaseSlot cardPurchasePrototype;
    [SerializeField] private GameObject cardParent;
    
    private ShopSelection _selection;

    private void Awake()
    {
        foreach (Transform c in cardParent.transform) 
            Destroy(c.gameObject);
    }
    
    private void Start()
    {
        _selection = new ShopSelectionPicker()
            .GenerateSelection(cards, equipment, party);
        _selection.Cards.ForEach(c => 
            Instantiate(cardPurchasePrototype, cardParent.transform)
                .Initialized(c));
    }
}
