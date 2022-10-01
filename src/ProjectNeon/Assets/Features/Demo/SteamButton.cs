using Steamworks;
using UnityEngine;
public class SteamButton : MonoBehaviour
{
    public void WishlistOnSteam()
    {
        AllMetrics.PublishInteractedWith("Wishlist Button");
        if (SteamManager.Initialized)
            SteamFriends.ActivateGameOverlayToWebPage("https://store.steampowered.com/app/1412960/Metroplex_Zero_SciFi_Card_Battler/");
        else
            Application.OpenURL("https://store.steampowered.com/app/1412960/Metroplex_Zero/");
    } 
}