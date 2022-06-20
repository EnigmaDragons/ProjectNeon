using UnityEngine;
using UnityEngine.UI;

public class ShopUIController : OnMessage<ToggleCardShop, ToggleEquipmentShop>
{
    [SerializeField] private GameObject cardShop;
    [SerializeField] private GameObject equipShop;
    [SerializeField] private ShopState shop;
    [SerializeField] private AllCorps allCorps;

    protected override void Execute(ToggleCardShop msg)
    {
        cardShop.SetActive(!cardShop.activeSelf);
        if (msg.IsTutorial)
            Message.Publish(new SetSuperFocusBuyControl(true));
    }

    protected override void Execute(ToggleEquipmentShop msg)
    {
        shop.Corp = allCorps.GetCorpByNameOrNone(msg.CorpName);
        equipShop.SetActive(!equipShop.activeSelf);
    }
}
