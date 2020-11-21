using UnityEngine;

public class CardShopPresenter : MonoBehaviour
{
    [SerializeField] private ShopCardPool cards;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopCardPurchaseSlot cardPurchasePrototype;
    [SerializeField] private GameObject cardParent;
 
    private ShopSelection _selection;
    private int _numCards;

    private void Awake()
    {
        _numCards = cardParent.transform.childCount;
        Clear();
    }

    private void Clear()
    {
        if (cardParent != null)
            foreach (Transform c in cardParent.transform)
                Destroy(c.gameObject);
    }

    private void Start() => GetMoreInventory();

    public void GetMoreInventory()
    {
        Clear();
        _selection = new ShopSelectionPicker()
            .GenerateCardSelection(cards, party, _numCards);
        _selection.Cards.ForEach(c => 
            Instantiate(cardPurchasePrototype, cardParent.transform)
                .Initialized(c));
    }
}
