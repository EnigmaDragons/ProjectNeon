using UnityEngine;

public class ShopUIController : OnMessage<ToggleCardShop, ToggleEquipmentShop>
{
    [SerializeField] private GameObject cardShop;
    [SerializeField] private GameObject equipShop;
    [SerializeField] private AdventureProgress2 adventure;

    protected override void Execute(ToggleCardShop msg) => cardShop.SetActive(!cardShop.activeSelf);
    protected override void Execute(ToggleEquipmentShop msg)
    {
        adventure.CorpShop = msg.Corp;
        equipShop.SetActive(!equipShop.activeSelf);
    }
}
