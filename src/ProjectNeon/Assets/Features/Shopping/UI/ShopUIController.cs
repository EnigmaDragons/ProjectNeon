using UnityEngine;

public class ShopUIController : OnMessage<ToggleCardShop, ToggleEquipmentShop>
{
    [SerializeField] private GameObject cardShop;
    [SerializeField] private GameObject equipShop;

    protected override void Execute(ToggleCardShop msg) => cardShop.SetActive(!cardShop.activeSelf);
    protected override void Execute(ToggleEquipmentShop msg) => equipShop.SetActive(!equipShop.activeSelf);
}
