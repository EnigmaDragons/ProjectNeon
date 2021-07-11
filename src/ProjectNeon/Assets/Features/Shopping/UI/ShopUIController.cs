using UnityEngine;

public class ShopUIController : OnMessage<ToggleCardShop, ToggleEquipmentShop>
{
    [SerializeField] private GameObject cardShop;
    [SerializeField] private GameObject equipShop;
    [SerializeField] private ShopState shop;

    protected override void Execute(ToggleCardShop msg) => cardShop.SetActive(!cardShop.activeSelf);
    protected override void Execute(ToggleEquipmentShop msg)
    {
        shop.Corp = msg.Corp;
        equipShop.SetActive(!equipShop.activeSelf);
    }
}
