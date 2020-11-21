using UnityEngine;

public class InitUniversalShopButtonForAdventure : MonoBehaviour
{
    [SerializeField] private GameObject universalShopButton;
    [SerializeField] private AdventureProgress adventure;

    private void Start() => universalShopButton.SetActive(adventure.Adventure.CanUseUniversalShop);
}
