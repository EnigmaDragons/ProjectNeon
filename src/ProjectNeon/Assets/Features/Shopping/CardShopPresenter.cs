using UnityEngine;

public class CardShopPresenter : MonoBehaviour
{
    [SerializeField] private ShopCardPool cards;
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private ShopCardPurchaseSlot cardPurchasePrototype;
    [SerializeField] private GameObject cardParent;
    [SerializeField] private AdventureProgress2 adventure;
 
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

    private void OnEnable() => GetMoreInventory();

    public void GetMoreInventory()
    {
        Clear();
        var selection = new ShopSelectionPicker(adventure.Stage, adventure.CurrentStage.RewardRarityFactors, party)
            .GenerateCardSelection(cards, _numCards);
        selection.Cards.ForEach(c => 
            Instantiate(cardPurchasePrototype, cardParent.transform)
                .Initialized(c));
    }
}
