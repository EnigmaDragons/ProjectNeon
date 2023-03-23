using UnityEngine;

public class ShopButtonHelper : MonoBehaviour
{
    public void Execute()
    {
        Message.Publish(new ToggleCardShop(false));
        Message.Publish(new NodeFinished());
    }
}