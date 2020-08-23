using UnityEngine;

public class ShopRestockingUI : OnMessage<PartyAdventureStateChanged>
{
    [SerializeField] private PartyAdventureState party;
    [SerializeField] private TextCommandButton restockButton;
    [SerializeField] private ShopPresenter shop;

    private void Awake()
    {
        restockButton.Init("What else you got?", Restock);
        restockButton.gameObject.SetActive(party.NumShopRestocks > 0);
    }

    private void Restock()
    {
        party.UpdateNumShopRestocksBy(-1);
        shop.GetMoreInventory();
    }

    protected override void Execute(PartyAdventureStateChanged msg)
    {
        restockButton.gameObject.SetActive(party.NumShopRestocks > 0);
    }
}
