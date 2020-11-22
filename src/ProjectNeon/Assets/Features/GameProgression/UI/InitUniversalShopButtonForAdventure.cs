using UnityEngine;

public class InitUniversalShopButtonForAdventure : MonoBehaviour
{
    [SerializeField] private GameObject universalShopButton;
    [SerializeField] private CurrentAdventure current;

    private void Start() => universalShopButton.SetActive(!current.Adventure.IsV2);
}
