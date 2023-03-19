using UnityEngine;

public class ShopUIController : OnMessage<ToggleCardShop>
{
    [SerializeField] private GameObject cardShop;

    protected override void Execute(ToggleCardShop msg)
    {
        cardShop.SetActive(!cardShop.activeSelf);
        if (cardShop.activeSelf && msg.IsTutorial)
            Message.Publish(new SetSuperFocusBuyControl(true));
    }
}
