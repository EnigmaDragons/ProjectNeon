using UnityEngine;

public class ShopUIController : OnMessage<ToggleCardShop, ToggleEquipmentShop>
{
    [SerializeField] private GameObject cardShop;
    [SerializeField] private GameObject equipShop;
    [SerializeField] private ShopState shop;
    [SerializeField] private AllCorps allCorps;

    protected override void Execute(ToggleCardShop msg)
    {
        cardShop.SetActive(!cardShop.activeSelf);
        if (cardShop.activeSelf)
            Message.Publish(new ShowTutorialByNameIfNeeded(Tutorials.Card));
    }

    protected override void Execute(ToggleEquipmentShop msg)
    {
        shop.Corp = allCorps.GetCorpByNameOrNone(msg.CorpName);
        equipShop.SetActive(!equipShop.activeSelf);
    }
}
