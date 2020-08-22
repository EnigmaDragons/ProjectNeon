using UnityEngine;

public class ShopUIController : OnMessage<ToggleShop>
{
    [SerializeField] private GameObject target;

    protected override void Execute(ToggleShop msg) => target.SetActive(!target.activeSelf);
}
